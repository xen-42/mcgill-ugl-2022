using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
   [Header("Movement")]
   [SerializeField] Transform orientation;
   public float moveSpeed = 20f;

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


    void ControlSpeed(){
        if (Input.GetKey(sprintkey) && isGrounded){
            moveSpeed = Mathf.Lerp(moveSpeed, runSpeed, acceleration * Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fastfov, fovaccel * Time.deltaTime);
        }else{
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, fovaccel * Time.deltaTime);
        }
    }
    private bool OnSlope(){
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight/2 + 0.5f)){
            if(slopeHit.normal != Vector3.up){
                return true;
            }else{
                return false;
            }
        }
        return false;
    }
    
   private void Start(){
       rb = GetComponent<Rigidbody>();
       rb.freezeRotation = true;
   }

   private void Update(){
       //Calls the player's input
       isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
         
       playerInput();
       playerDrag();
       ControlSpeed();

       if (Input.GetKeyDown(jumpKey) && isGrounded){
           Jump();
       }

       slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
   }

   void Jump(){
       rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
       rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
   }

   void playerInput(){
       horizontalMovement = Input.GetAxisRaw("Horizontal");
       verticalMovement = Input.GetAxisRaw("Vertical");

       moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
   }

   void playerDrag(){
       if (isGrounded){
            rb.drag = rbDrag;
        }
        else{
            rb.drag = airDrag;
        }
   }

   
   private void FixedUpdate(){
       MovePlayer();
   }

   void MovePlayer(){
       if(isGrounded && !OnSlope()){
        rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
       }
       else if(isGrounded && OnSlope()){
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);

       }
       else if (!isGrounded){
           rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
       }
      

   }
}
