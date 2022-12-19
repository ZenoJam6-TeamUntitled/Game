using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAwayCC : MonoBehaviour
{
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();    
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = Vector3.right * 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = -Vector3.right * 1;
        }        
    }
}
