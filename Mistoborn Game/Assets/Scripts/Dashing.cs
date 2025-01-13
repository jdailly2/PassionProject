using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Dashing : MonoBehaviour
{

    [Header("References")] 
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Dashing")] 
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;
    public float maxDashYSpeed;

    [Header("CameraEffects")] 
    public MouseLook cam;
    public float dashFov;
    
    
    [Header("Cooldown")] 
    public float dashCd;
    public float dashCdTimer;

    [Header("Settings")] 
    public bool useCameraForward = true;
    public bool allowAllDirections = true;//This setting is used to allow the play to dash in any direction
    public bool disableGravity = false;//This setting is used to turn off gravity when dashing
    public bool resetVel = true;//This setting is used to set the players velocity to zero right before they dash

    [Header("Input")] 
    public KeyCode dashKey = KeyCode.R;

    private Vector3 delayedForceToApply;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(dashKey))
        {
            Dash();
        }

        if (dashCdTimer > 0)
        {
            dashCdTimer -= Time.deltaTime;
        }
    }

    //Function that applies the dash force
    private void Dash()
    {
        if (dashCdTimer > 0)
        {
            return;
        }
        else
        {
            dashCdTimer = dashCd;
        }
        
        pm.dashing = true;
        pm.maxYSpeed = maxDashYSpeed;
        cam.DoFov(dashFov);
        Transform forwardT;

        if (useCameraForward)
        {
            forwardT = playerCam;
        }
        else
        {
            forwardT = orientation;
        }

        Vector3 direction = GetDirection(forwardT);
        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
        {
            rb.useGravity = false;
        }
        
        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce),0.025f);
        
        Invoke(nameof(ResetDash), dashDuration);
    }

    //Function delays when the force gets added the player
    private void DelayedDashForce()
    {
        if (resetVel)
        {
            rb.linearVelocity = Vector3.zero;
        }
        rb.AddForce(delayedForceToApply,ForceMode.Impulse);
    }
    
    //Function that resets the dash 
    private void ResetDash()
    {
        pm.dashing = false;
        pm.maxYSpeed = 0;
        cam.DoFov(85f);
        if (disableGravity)
        {
            rb.useGravity = true;
        }
    }

    //Function determines what direction the dash should go
    private Vector3 GetDirection(Transform forwardT)
    {
       float horizontalInput = Input.GetAxisRaw("Horizontal");
       float verticalInput = Input.GetAxisRaw("Vertical");

       Vector3 direction = new Vector3();

       if (allowAllDirections)
       {
           direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
       }
       else
       {
           direction = Vector3.forward;
       }

       if (verticalInput == 0 && horizontalInput == 0)
       {
           direction = forwardT.forward;
       }

       return direction.normalized;
    }
}
