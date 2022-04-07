using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static InputManager;

public class Player : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] public Transform orientation;
    [SerializeField] public float moveSpeed = 6f;

    [Header("Drag")]
    private float rbDrag = 6f;
    private float airDrag = 2f;
    private float movementMultiplier = 10f;
    [SerializeField] private float airMultiplier = 0.4f;
    private float playerHeight = 2f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    private float groundDistance = 0.4f;
    private bool isGrounded;

    [Header("Jumping")]
    public float jumpForce = 5f;

    private RaycastHit slopeHit;

    [Header("Camera Adjusts")]
    [SerializeField] public Camera cam;
    [SerializeField] private float fastfov;
    [SerializeField] private float fov;
    [SerializeField] private float fovaccel;
    public static float sensX;
    public static float sensY;
    private float multiplier = 0.01f;
    public float xRotation;
    public float yRotation;

    [Header("Sprinting")]
    [SerializeField] public float walkSpeed = 4f;
    [SerializeField] public float runSpeed = 6f;
    [SerializeField] public float acceleration = 10f;

    [Header("Interacting")]
    [SerializeField] private Transform heldItemPosition;
    [SerializeField] private float heldItemTranslationResponsiveness = 20f;
    [SerializeField] private float heldItemRotationResponsiveness = 10f;

    [Header("Physics Stuff")]
    [SerializeField] public Rigidbody rb;
    [SerializeField] public new Collider collider;

    private Interactable _focusedObject;
    public Holdable heldObject;

    public float stressModifier;

    // Synced network stuff
    [SyncVar] public Vector3 movement;
    [SyncVar] public bool jump;
    [SyncVar] public bool sprint;
    [SyncVar] private float _serverSideStressModifier;

    public static Player Instance { get; private set; }
    public static Player OtherPlayer { get; private set; }

    // Customization options
    [SyncVar] public PlayerCustomization.PLANT plant;
    [SyncVar] public PlayerCustomization.DRINK drink;
    [SyncVar] public PlayerCustomization.POSTER poster;
    [SyncVar] public PlayerCustomization.COLOUR colour;

    private bool _interactedThisTick = false;

    public string displayName;

    private void Start()
    {
        // We do this because we create the player in the lobby scene then put them in the play scene
        DontDestroyOnLoad(this);

        SceneManager.sceneLoaded += OnSceneLoaded;

        rb.freezeRotation = true;

        if (hasAuthority)
        {
            // This is the clients player so they will use it's camera
            cam.tag = "MainCamera";
            InputManager.CurrentInputMode = InputManager.InputMode.Player;
            Instance = this;
        }
        else
        {
            // This is the other player (there can only be two)
            OtherPlayer = this;
            GameObject.Destroy(cam.GetComponent<AudioListener>());
            cam.enabled = false;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Just changed scenes so we go back to being destroyed on load
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
    }

    private void Update()
    {
        if (hasAuthority)
        {
            CheckInputs();
        }

        // Move held items here so they go smoothly (since we disable their collisions its fine that it isnt on server)
        if (heldObject != null)
        {
            var targetPosition = heldItemPosition.position;
            var dir = (heldItemPosition.position - transform.position);

            if (Physics.Raycast(transform.position, dir.normalized, out var hit, dir.magnitude, ~LayerMask.NameToLayer("Player")))
            {
                targetPosition = hit.point - dir.normalized * 0.5f;
            }

            heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, targetPosition, Time.deltaTime * heldItemTranslationResponsiveness);
            heldObject.transform.rotation = Quaternion.Lerp(heldObject.transform.rotation, heldItemPosition.rotation, Time.deltaTime * heldItemRotationResponsiveness);
        }
    }

    [Client]
    private void CheckInputs()
    {
        // We only want to check input on the objects we have authority for
        if (!hasAuthority) return;

        // If we're paused or in a minigame we cant control the player
        if (InputManager.CurrentInputMode != InputManager.InputMode.Player)
        {
            // Don't move during minigame
            if (this.movement != Vector3.zero)
            {
                if (isServer)
                {
                    this.movement = Vector3.zero;
                }
                else
                {
                    CmdStopMoving();
                }
            }

            // If we were looking at something make sure its lost focus
            if (_focusedObject != null)
            {
                foreach (var interactable in _focusedObject.GetComponents<Interactable>())
                {
                    interactable.LoseFocus();
                }
                _focusedObject = null;
            }

            return;
        }

        // Physics stuff
        var inputMovement = InputManager.GetMovementAxis();
        var movement = orientation.forward * inputMovement.y + orientation.right * inputMovement.x;

        var jump = InputManager.IsCommandPressed(InputManager.InputCommand.Jump);

        var sprint = InputManager.IsCommandPressed(InputManager.InputCommand.Sprint);

        // Camera
        var lookVector = InputManager.GetLookAxis();

        yRotation += lookVector.x * sensX * multiplier;
        xRotation -= lookVector.y * sensY * multiplier;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        CmdSendInputs(movement, jump, sprint, xRotation, yRotation, stressModifier);

        // The client can set the rotations immediately
        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        // Raycast for object interaction / pickup

        GameObject hitObject = null;
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out RaycastHit hit, 3f))
        {
            hitObject = hit.collider.gameObject;
        }

        // We were looking at an interactable object but now we aren't
        if (_focusedObject != null && hitObject != _focusedObject)
        {
            _focusedObject.LoseFocus();

            _focusedObject = null;
        }

        // If we just started looking at it
        if (hitObject != null && hitObject != _focusedObject)
        {
            var interactable = hitObject.GetComponent<Interactable>();
            if (interactable != null)
            {
                _focusedObject = interactable;
                interactable.GainFocus();
            }
        }

        // Debug
        if (Keyboard.current[Key.F11].wasPressedThisFrame)
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        if (_focusedObject != null && InputManager.IsCommandJustPressed(InputCommand.Interact))
        {
            if (_focusedObject.Interact()) _interactedThisTick = true;
        }

        // Held item
        if (heldObject != null && !_interactedThisTick)
        {
            if (InputManager.IsCommandJustPressed(InputCommand.PickUp))
            {
                if (isServer)
                {
                    RpcDrop();
                }
                else
                {
                    Player.Instance.DoWithAuthority(heldObject.netIdentity, CmdDrop);
                }
            }

            if (InputManager.IsCommandJustPressed(InputCommand.Throw))
            {
                if (isServer)
                {
                    // Dropping removes our reference to the object but we need that
                    var obj = heldObject;
                    RpcDrop();
                    obj.Toss(cam.transform.forward.normalized);
                }
                else
                {
                    var obj = heldObject;
                    CmdDrop();
                    Player.Instance.DoWithAuthority(netIdentity, () =>
                    {
                        obj.CmdToss(cam.transform.forward.normalized);
                    });
                }
            }
        }

        _interactedThisTick = false;
    }

    private void FixedUpdate()
    {
        var actualMoveSpeed = Mathf.Lerp(moveSpeed, 1, _serverSideStressModifier * _serverSideStressModifier);

        isGrounded = Physics.Raycast(groundCheck.position, -Vector3.up, groundDistance + 0.1f);
        PlayerDrag();
        ControlSpeed();

        if (jump && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            jump = false;
        }

        var slopeMoveDirection = Vector3.ProjectOnPlane(movement, slopeHit.normal);

        if (isGrounded && !OnSlope())
        {
            rb.AddForce(movement.normalized * actualMoveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * actualMoveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(movement.normalized * actualMoveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }

    private void ControlSpeed()
    {
        var actualMoveSpeed = Mathf.Lerp(moveSpeed, 1, _serverSideStressModifier * _serverSideStressModifier);
        var actualWalkSpeed = Mathf.Lerp(walkSpeed, 1, _serverSideStressModifier * _serverSideStressModifier);
        var actualRunSpeed = Mathf.Lerp(runSpeed, 1, _serverSideStressModifier * _serverSideStressModifier);
        var actualAcceleration = Mathf.Lerp(acceleration, 1, _serverSideStressModifier * _serverSideStressModifier);

        if (sprint && isGrounded)
        {
            moveSpeed = Mathf.Lerp(actualMoveSpeed, actualRunSpeed, actualAcceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(actualMoveSpeed, actualWalkSpeed, actualAcceleration * Time.deltaTime);
        }

        if (sprint && isGrounded && movement != Vector3.zero)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fastfov, fovaccel * Time.deltaTime);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, fovaccel * Time.deltaTime);
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void PlayerDrag()
    {
        if (isGrounded)
        {
            rb.drag = rbDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    #region Commands and RPC

    [Command]
    public void CmdSendInputs(Vector3 movement, bool jump, bool sprint, float xRot, float yRot, float stress)
    {
        RpcSendInputs(movement, jump, sprint, xRot, yRot, stress);
        xRotation = xRot;
        yRotation = yRot;
    }

    [ClientRpc]
    private void RpcSendInputs(Vector3 movement, bool jump, bool sprint, float xRot, float yRot, float stress)
    {
        this.movement = movement;
        this.jump = jump;
        this.sprint = sprint;
        _serverSideStressModifier = stress;

        if (!hasAuthority)
        {
            // We're rotating the other character
            cam.transform.localRotation = Quaternion.Euler(xRot, yRot, 0);
            orientation.transform.rotation = Quaternion.Euler(0, yRot, 0);
            yRotation = yRot;
            xRotation = xRot;
        }
    }

    [Command]
    public void CmdGrab(Holdable target)
    {
        RpcGrab(target);
    }

    [ClientRpc]
    public void RpcGrab(Holdable target)
    {
        target.Grab(this);
        heldObject = target;
    }

    [Command]
    public void CmdDrop()
    {
        RpcDrop();
    }

    [ClientRpc]
    public void RpcDrop()
    {
        if (heldObject != null)
        {
            heldObject.Drop();
            heldObject = null;
        }
    }

    public void DoWithAuthority(NetworkIdentity identity, Action action)
    {
        if (!NetworkClient.active) return;

        if (identity != null && !identity.hasAuthority)
        {
            CmdGetAuthority(identity);

            ActionManager.RunWhen(
                // Run when we get authority or when the server stops
                () => netIdentity.hasAuthority || !NetworkClient.active,
                // If the server stopped don't try to call the action
                () => { if (NetworkClient.active) action.Invoke(); }
            );
        }
        else
        {
            action.Invoke();
        }
    }

    [Command]
    private void CmdGetAuthority(NetworkIdentity identity)
    {
        try
        {
            if (identity != null && !identity.hasAuthority)
            {
                identity.RemoveClientAuthority();
                identity.AssignClientAuthority(connectionToClient);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}: {e.StackTrace}");
        }
    }

    [Command]
    public void CmdStopMoving()
    {
        movement = Vector3.zero;
    }

    #endregion Commands and RPC

    #region Override

    public override void OnStartClient()
    {
        CustomNetworkManager.Instance.players.Add(this);

        base.OnStartClient();
    }

    public override void OnStopClient()
    {
        CustomNetworkManager.Instance.players.Remove(this);

        base.OnStopClient();
    }

    #endregion Override
}