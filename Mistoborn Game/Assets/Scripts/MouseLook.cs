using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;


public class MouseLook : MonoBehaviour
{

    public float mouseSensitivity = 100f;

    public Transform orientation;

    private float xRotation = 0f;
    private float yRotation = 0f;
    void Start()
    {
        //Locks cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

   private void Update()
    {
        //Time. deltaTime ensures that movment will stay consistent between all frame rates
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y")* mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;
        //Clamp makes sure the user can not look more that 90 degrees on the x axis
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        
        //Quaternion is responsible for rotaions in unity
        transform.rotation = Quaternion.Euler(xRotation,yRotation,0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);


    }
}