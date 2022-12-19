using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    [SerializeField] ParticleSystem inRangeEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player has entered Grapple Range.");
            inRangeEffect.Play();
            GrappleTool.instance.isWithinGrappleRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player has exited Grapple Range.");
            inRangeEffect.Stop();
            GrappleTool.instance.isWithinGrappleRange = false;
            GrappleTool.instance.grappleLine.enabled = false;
            GrappleTool.instance.isGrappling = false;
        }
    }
}
