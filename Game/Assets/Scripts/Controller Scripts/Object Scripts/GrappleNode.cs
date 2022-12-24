using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrappleNode : MonoBehaviour
{
    public ParticleSystem GPPS;
    public string playerTag = "Player";
    public Canvas GrapplePrompt;
    public GameObject player;

    private void Start()
    {
         ParticleSystem.MainModule main = GPPS.main;
         main.playOnAwake = false;
    }

    void OnTriggerEnter(Collider thingEnter)
    {
            if(thingEnter.gameObject.tag == "Player")
            {
                GPPS.Play();
            }
    }
    void OnTriggerExit(Collider thingExit)
    {
            if(thingExit.gameObject.tag == "Player")
            {
                GPPS.Stop();
            }
    }
}





