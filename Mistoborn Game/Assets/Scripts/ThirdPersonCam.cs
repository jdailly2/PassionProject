using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ThirdPersonCam : MonoBehaviour
{

    [Header("References")] 
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    public CameraStyle currentStyle;

    public float rotationSpeed;

    public Transform combatLookAt;
    public enum CameraStyle
    {
        Basic,
        Combat,
        TopDown
    }
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, player.position.z);
        orientation.forward = viewDir.normalized;

        if (currentStyle == CameraStyle.Basic)
        {
            //rotate player object
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }else if (currentStyle == CameraStyle.Combat)
            {
                
            } 
        }else if (currentStyle == CameraStyle.Combat)
        {
            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            playerObj.forward = dirToCombatLookAt.normalized;

        }
        
      


    }
}
