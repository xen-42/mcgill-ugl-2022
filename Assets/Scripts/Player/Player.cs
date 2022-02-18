using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Vector3 moveDirection;
    Vector3 slopeMoveDirection;
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

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float acceleration = 10f;
    bool isJumping;

    [Header("Interacting")]
    [SerializeField] GameObject heldItemPosition;

    private GameObject _interactableObject;
    private GameObject _heldObject;

    void ControlSpeed()
    {
        if (InputManager.IsCommandPressed(InputManager.InputCommand.Sprint) && isGrounded)
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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if(hasAuthority)
        {
            // This is the clients player so they will use it's camera
            cam.tag = "MainCamera";
        }
        else
        {
            cam.enabled = false;
        }
    }

    [Client]
    private void Update()
    {
        // We only want to check input on the objects we have authority for
        if (!hasAuthority) return;

        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        isGrounded = Physics.Raycast(groundCheck.position, -Vector3.up, groundDistance + 0.1f);

        //Calls the player's input
        PlayerInput();
        
        PlayerDrag();
        ControlSpeed();

        if (InputManager.IsCommandPressed(InputManager.InputCommand.Jump) && isGrounded)
        {
            isJumping = true;
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    [Client]
    private void FixedUpdate(){
        if (!hasAuthority) return;
        MovePlayer();
        if (isJumping){
            Jump();
        }
    }


    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        isJumping = false;
    }

    void PlayerInput()
    {
        var movement = InputManager.GetMovementAxis();

        moveDirection = orientation.forward * movement.y + orientation.right * movement.x;

        if (_heldObject != null && InputManager.IsCommandJustPressed(InputManager.InputCommand.PickUp))
        {
            CmdDrop();
        }

        // Raycast for object interaction / pickup
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out RaycastHit hit, 3f))
        {
            var hitObject = hit.collider.gameObject;
            if (_interactableObject != hitObject && hitObject.GetComponent<Interactable>() != null)
            {
                _interactableObject = hit.collider.gameObject;
                EventManager.Instance.TriggerEvent("InteractableObjectHit");
            }

            var holdable = hitObject.GetComponent<Holdable>();
            if (holdable != null && _heldObject != hitObject)
            {
                if (InputManager.IsCommandJustPressed(InputManager.InputCommand.PickUp))
                {
                    CmdGrab(hitObject);
                }
            }
        }
        else
        {
            if (_interactableObject != null)
            {
                _interactableObject = null;
                EventManager.Instance.TriggerEvent("InteractableObjectLost");
            }
        }
    }

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

    void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);

        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }

    [Command]
    public void CmdGrab(GameObject target)
    {
        Debug.Log("Grabbing");
        _heldObject = target;
        _heldObject.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        _heldObject.GetComponent<Holdable>().Parent = gameObject;
    }

    [Command]
    public void CmdDrop()
    {
        Debug.Log("Dropping");
        _heldObject.GetComponent<Holdable>().Parent = null;
        _heldObject = null;
    }
}
