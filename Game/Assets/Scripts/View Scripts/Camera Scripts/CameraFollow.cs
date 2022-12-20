using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The object to follow
    public float smoothTime = 0.3f; // The time it takes for the camera to catch up to the target
    public Vector3 offset; // The distance between the camera and the target

    private Vector3 velocity = Vector3.zero; // The camera's current velocity

    private void Update()
    {
        // Use the SmoothDamp function to smoothly transition the camera's position to the target's position
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smoothTime);
    }
}
