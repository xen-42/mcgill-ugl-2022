using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] Transform orientation;
    public float moveSpeed = 6f;

    [Header("Drag")]
    float rbDrag = 6f;
    float airDrag = 2f;
    float horizontalMovement;
    float verticalMovement;
    float movementMultiplier = 10f;
    [SerializeField] float airMultiplier = 0.4f;
    float playerHeight = 2f;

    [Header("Ground Detection")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform groundCheck;
    float groundDistance = 0.4f;
    bool isGrounded;

    Rigidbody rb;
    [Header("Jumping")]
    public float jumpForce = 5f;

    RaycastHit slopeHit;
    [Header("Camera Adjusts")]
    [SerializeField] private Camera cam;
    [SerializeField] private float fastfov;
    [SerializeField] private float fov;
    [SerializeField] private float fovaccel;
    [SerializeField] public float sensX;
    [SerializeField] public float sensY;
    float multiplier = 0.01f;
    float xRotation;
    float yRotation;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float acceleration = 10f;

    [Header("Interacting")]
    [SerializeField] Transform heldItemPosition;

    private GameObject _focusedObject;
    public Holdable heldObject;

    // Synced network stuff
    [SyncVar] private Vector3 _movement;
    [SyncVar] private bool _jump;
    [SyncVar] private bool _sprint;

    public static Player Instance { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
            cam.enabled = false;
        }
    }

    private void Update()
    {
        // We only check for inputs in here

        // We only want to check input on the objects we have authority for
        if (!hasAuthority) return;

        // If we're paused or in a minigame we cant control the player
        if (InputManager.CurrentInputMode != InputManager.InputMode.Player)
        {
            // If we were looking at something make sure its lost focus
            if(_focusedObject != null)
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

        CmdSendInputs(movement, jump, sprint, xRotation, yRotation);

        // Raycast for object interaction / pickup

        GameObject hitObject = null;
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out RaycastHit hit, 3f))
        {
            hitObject = hit.collider.gameObject;
        }

        // We were looking at an interactable object but now we aren't
        if (_focusedObject != null && hitObject != _focusedObject)
        {
            foreach (var interactable in _focusedObject.GetComponents<Interactable>())
            {
                interactable.LoseFocus();
            }
            _focusedObject = null;
        }

        // If we just started looking at it
        if (hitObject != null && hitObject != _focusedObject)
        {
            _focusedObject = hitObject;
            foreach (var interactable in hitObject.GetComponents<Interactable>())
            {
                interactable.GainFocus();
            }
        }

        // Debug
        if (Keyboard.current[Key.F11].wasPressedThisFrame)
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    private void FixedUpdate()
    {
        if (!isServer) return;

        isGrounded = Physics.Raycast(groundCheck.position, -Vector3.up, groundDistance + 0.1f);
        PlayerDrag();
        ControlSpeed();

        if (_jump && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            _jump = false;
        }

        var slopeMoveDirection = Vector3.ProjectOnPlane(_movement, slopeHit.normal);

        if (isGrounded && !OnSlope())
        {
            rb.AddForce(_movement.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);

        }
        else if (!isGrounded)
        {
            rb.AddForce(_movement.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }

        if (heldObject != null)
        {
            heldObject.transform.position = heldItemPosition.transform.position;
            heldObject.transform.rotation = heldItemPosition.transform.rotation;
        }
    }

    [Server]
    void ControlSpeed()
    {
        if (_sprint && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, runSpeed, acceleration * Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fastfov, fovaccel * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, fovaccel * Time.deltaTime);
        }
    }

    [Server]
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

    [Server]
    void PlayerDrag()
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

    [Command]
    public void CmdSendInputs(Vector3 movement, bool jump, bool sprint, float xRot, float yRot)
    {
        RpcSendInputs(movement, jump, sprint, xRot, yRot);
    }

    [ClientRpc]
    private void RpcSendInputs(Vector3 movement, bool jump, bool sprint, float xRot, float yRot)
    {
        _movement = movement;
        _jump = jump;
        _sprint = sprint;

        cam.transform.localRotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRot, 0);
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
    private void RpcDrop()
    {
        heldObject.Drop();
        heldObject = null;
    }

    [Command]
    public void CmdGiveAuthority(NetworkIdentity identity)
    {
        if (!identity.hasAuthority)
        {
            identity.RemoveClientAuthority();
            identity.AssignClientAuthority(connectionToClient);
        }
    }
}
