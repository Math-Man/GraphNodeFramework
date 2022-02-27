using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    [SerializeField] private float speedMult;
    private Vector3 movementVector;
    
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            movementVector += new Vector3(-speedMult, 0, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movementVector += new Vector3(speedMult, 0, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            movementVector += new Vector3(0, 0,speedMult);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movementVector += new Vector3(0, 0, -speedMult);
        }
        
        transform.position += movementVector;
        movementVector *= 0.9f;
    }
}
