using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHurtboxing : MonoBehaviour {

    public Vector3 appliedForce = new Vector3(0, 1, 0);
    public float modForce = 5f;
    public float lifeSpan = 2f;

    private PlayerController playerScript;
    private bool timeCheck;
    private float timeWait;

    void Start()
    {
        GameObject player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();
        timeWait = Time.time;
    }

    void Update()
    {
        Destroy(gameObject, lifeSpan);
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Enemy") // If the hit object is an enemy : force and damage calculation goes here
        {
            other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            other.gameObject.GetComponent<Rigidbody>().AddForce(appliedForce * modForce * playerScript.facingSide, ForceMode.Impulse);
        }
    } // Called whenever something gets in the instantiated hurtbox
}
