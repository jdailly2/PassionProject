using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraTracker : MonoBehaviour
{

    public Transform camerPostion;
   private void Update()
   {
       transform.position = camerPostion.position;
   }
}
