using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleNode : MonoBehaviour
{
    public ParticleSystem GPPS;
    public float radius = 10f;
    public string playerTag = "Player";
    public bool nowEmitting = false; 

    public GameObject player;

    private void Start()
    {
         ParticleSystem.MainModule main = GPPS.main;
         main.playOnAwake = false;
    }

    private void Update()
    {

        RaycastHit hit;
        bool hitPlayer = Physics.SphereCast(transform.position, radius, player.transform.position - transform.position, 
        out hit, Mathf.Infinity);
        if (hitPlayer && hit.collider.tag == playerTag)
        {
            GPPS.Play();
        }
        else
        {
            GPPS.Stop();
        }
    }
}





