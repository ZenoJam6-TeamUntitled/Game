using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleTool : MonoBehaviour
{
    public static GrappleTool instance;

    FixedJoint fixedJoint;

    [SerializeField] GameObject grappleTool;    
    [SerializeField] GrapplePoint grapplePoint;
    public LineRenderer grappleLine;

    public bool isWithinGrappleRange;
    public bool isGrappling;

    [SerializeField] float ascensionSpeed = 10f;

    void Awake()
    {
        instance = this;

        fixedJoint = GetComponent<FixedJoint>();
    }

    void Update()
    {
        Grapple();

        if (isGrappling && Input.GetKey(KeyCode.W))
        {
            Ascend();
        }
        
        if (isGrappling && Input.GetKey(KeyCode.S))
        {
            Descend();
        }
    }

    void Ascend()
    {
        //fixedJoint.maxDistance -= ascensionSpeed * Time.deltaTime;
        GetComponentInParent<Rigidbody>().position = fixedJoint.anchor;
    }
    
    void Descend()
    {
        //fixedJoint.maxDistance += ascensionSpeed * Time.deltaTime;
        GetComponentInParent<Rigidbody>().position = fixedJoint.anchor;
    }

    void Grapple()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isWithinGrappleRange)
            {
                return;
            }
            else
            {
                Debug.Log("Grapple shot.");

                //line renderer
                grappleLine.enabled = true;
                isGrappling = true;
            }
        }

        if (Input.GetKey(KeyCode.Space) && isGrappling)
        {
            //spring joint
            fixedJoint.connectedBody = grapplePoint.GetComponent<Rigidbody>(); //this needs to be set dynamically, not just dragged over in the inspector
            fixedJoint.anchor = grappleTool.GetComponent<Rigidbody>().position;
            fixedJoint.connectedAnchor = grapplePoint.GetComponent<Rigidbody>().position;
            //fixedJoint.maxDistance = Vector3.Distance(grappleTool.transform.position, grapplePoint.transform.position);

            //line renderer
            grappleLine.SetPosition(0, grappleTool.transform.position);
            grappleLine.SetPosition(1, grapplePoint.transform.position);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            //spring joint
            fixedJoint.connectedBody = null;
            fixedJoint.connectedAnchor = Vector3.zero; ;
            //fixedJoint.maxDistance = 0;

            //line renderer
            grappleLine.enabled = false;
            isGrappling = false;
        }
    }
}
