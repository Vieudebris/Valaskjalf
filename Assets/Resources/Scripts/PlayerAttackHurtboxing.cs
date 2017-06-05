using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAttackHurtboxing : NetworkBehaviour
{
    public Vector3 appliedForce = new Vector3(0, 1, 0);
    public float modForce = 5f;
    public float lifeSpan = 2f;

    private PlayerController playerScript;
    private bool timeCheck;
    private float timeWait;

    void Start()
    {
        //GameObject player = GameObject.Find("Player");
        GameObject player = GameObject.Find("Player_Network(Clone)");  //For the network
        playerScript = player.GetComponent<PlayerController>();
        timeWait = Time.time;
    }

    void Update()
    {
        Destroy(gameObject, lifeSpan/60);
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Enemy") // If the hit object is an enemy : force and damage calculation goes here
        {
            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.gameObject.GetComponent<Rigidbody>().AddForce(appliedForce * modForce * playerScript.facingSide, ForceMode.Impulse);
        }
    } // Called whenever something gets in the instantiated hurtbox
}
