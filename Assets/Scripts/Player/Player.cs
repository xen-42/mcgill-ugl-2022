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
    [Header("Keybind")]
    [SerializeField] KeyCode sprintkey = KeyCode.LeftShift;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

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
    void ControlSpeed()
    {
        if (Input.GetKey(sprintkey) && isGrounded)
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

        //Calls the player's input
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        PlayerInput();
        
        PlayerDrag();
        ControlSpeed();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
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
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
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
}
