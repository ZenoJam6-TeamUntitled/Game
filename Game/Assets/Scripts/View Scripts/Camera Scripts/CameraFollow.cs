using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
     public float FollowSpeed = 2f;
    public float yOffset =1f;
    public Transform target;

    public float maxYPos;
    public float minYPos;
    public float yPos;

    // Update is called once per frame
    void Update()
    {
        if (target.position.y < minYPos) yPos = minYPos;
        else if (target.position.y > maxYPos) yPos = maxYPos;
        else yPos =target.position.y + yOffset;


        Vector3 newPos = new Vector3(0,yPos,-10f);
        transform.position = Vector3.Slerp(transform.position,newPos,FollowSpeed*Time.deltaTime);
    }
}
