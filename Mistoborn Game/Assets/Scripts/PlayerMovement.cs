using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] 
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    [Header("Crouching")] 
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    
    
    [Header("Ground Check")] 
    public float groundDrag;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public float groundDistance = 0.4f;
    private bool grounded;

    [Header("Slope Handling")] 
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope = false;
    

    [Header("Jump movement")] 
    public float jumpForce;
    public float jumpCooldown;
    public float airMultipler;
    private bool readyToJump = true;

    [Header("Keybinds")] 
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    
    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;

    //variable that holds the state the player is in
    public MovementState state;
    
    //Contains the different states the player can be in
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }
    

     private void Start()
     {
         rb = GetComponent<Rigidbody>();
         rb.freezeRotation = true;
         startYScale = transform.localScale.y;

     }


     void Update()
    {
        //checks to see if the player is on the ground
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance,whatIsGround);
        
        MyInput();
        SpeedControl();
        StateHandler();
        Debug.Log(state);
        
        //adds drag to the player when on the ground
        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }

      
    }

     private void FixedUpdate()
     {
         MovePlayer();
     }

     //function changes the state of the user
     private void StateHandler()
     {

         //Mode - crouching
         if (Input.GetKey(crouchKey))
         {
             state = MovementState.crouching;
             moveSpeed = crouchSpeed;
         }
         //Mode - Sprinting
         else if (grounded && Input.GetKey(sprintKey))
         {
             state = MovementState.sprinting;
             moveSpeed = sprintSpeed;
         }else if (grounded)
         {
             state = MovementState.walking;
             moveSpeed = walkSpeed;
         }
         else
         {
             state = MovementState.air;
             
         }
         
         
     }
     

     //This function gets the inputs from the player
     private void MyInput()
     {
         horizontalInput = Input.GetAxisRaw("Horizontal");
         verticalInput = Input.GetAxisRaw("Vertical");
         
         //when to jump
         if (Input.GetKey(jumpKey) && readyToJump && grounded)
         {
             readyToJump = false;
             Jump();
             Invoke(nameof(ResetJump),jumpCooldown);
         }

         //Start crouching
         if (Input.GetKeyDown(crouchKey))
         {
             transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
             rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
         }
         
         //Stop crouching
         if (Input.GetKeyUp(crouchKey))
         {
             transform.localScale = new Vector3(transform.localScale.x,startYScale , transform.localScale.z);
         }
         
         
     }


     //This function adds the movment force of where the player should go
     private void MovePlayer()
     {
         moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

         if (OnSlope() && !exitingSlope)
         {
        
             rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f,ForceMode.Force);
             if (rb.linearVelocity.y > 0)
             {
                 rb.AddForce(Vector3.down * 80f,ForceMode.Force);
             } 
             
         }
         
         if (grounded)
         {
             rb.AddForce(moveDirection.normalized * moveSpeed* 10f,ForceMode.Force);
         }
         else if (!grounded)
         {
             rb.AddForce(moveDirection.normalized * moveSpeed* 10f* airMultipler,ForceMode.Force);
         }

     }

     
     //This function controls the speed the player can go
     private void SpeedControl()
     {
         //if the player is standing on a slope we want to limit there speed
         if (OnSlope() && !exitingSlope)
         {
             if (rb.linearVelocity.magnitude > moveSpeed)
             {
                 rb.linearVelocity =  rb.linearVelocity.normalized* moveSpeed;
             }
         }
         else
         {
             Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

             if (flatVel.magnitude > moveSpeed)
             {
                 Vector3 limitedVal = flatVel.normalized * moveSpeed;
                 rb.linearVelocity = new Vector3(limitedVal.x, rb.linearVelocity.y, limitedVal.z);


             }
         }
         
         
        
         

     }

     //function that applies upward force when player wants to jump
     private void Jump()
     {
         exitingSlope = true;
         
         rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
         rb.AddForce(transform.up * jumpForce,ForceMode.Impulse);

     }

     //function that resets the jump
     private void ResetJump()
     {
         readyToJump = true;
         exitingSlope = false;
     }

     //checks to see if the player is standing on a slope and returns true if so
     private bool OnSlope()
     {
         
         if (Physics.Raycast(groundCheck.position, Vector3.down, out slopeHit, 0.3f))
         {
            
             float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
             return angle < maxSlopeAngle && angle != 0 ;
         }

         return false;
     }

     //Function finds the agnle of the slope.
     private Vector3 GetSlopeMoveDirection()
     {
         return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
     }
     
     
}
