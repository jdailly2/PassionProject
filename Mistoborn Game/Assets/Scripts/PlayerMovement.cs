using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] 
    public float moveSpeed;
    
    [Header("Ground Check")] 
    public float groundDrag;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public float groundDistance = 0.4f;
    private bool grounded;

    [Header("Jump movement")] 
    public float jumpForce;
    public float jumpCooldown;
    public float airMultipler;
    private bool readyToJump = true;

    [Header("Keybinds")] 
    public KeyCode jumpKey = KeyCode.Space;
    
    
    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;
    
    
    

     private void Start()
     {
         rb = GetComponent<Rigidbody>();
         rb.freezeRotation = true;
     }


     void Update()
    {
        //checks to see if the player is on the ground
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance,whatIsGround);
        
        MyInput();
        SpeedControl();
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
         
     }


     //This function adds the movment force of where the player should go
     private void MovePlayer()
     {
         moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
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
         Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

         if (flatVel.magnitude > moveSpeed)
         {
             Vector3 limitedVal = flatVel.normalized * moveSpeed;
             rb.linearVelocity = new Vector3(limitedVal.x, rb.linearVelocity.y, limitedVal.z);


         }
         

     }

     //function that applies upward force when player wants to jump
     private void Jump()
     {
         rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
         rb.AddForce(transform.up * jumpForce,ForceMode.Impulse);

     }

     //function that resets the jump
     private void ResetJump()
     {
         readyToJump = true;
     }
}
