using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAttackHurtboxing : NetworkBehaviour
{
    public Vector3 appliedForce = new Vector3(0, 1, 0);
    public float modForce = 5f;
    public float lifeSpan = 2f;
    public int hbSet;

    private PlayerController playerScript;
    private EnemyBehaviour enemyScript;
    private bool timeCheck;
    private float timeWait;

    public bool knockdown;
    public int damage;
    public int meterValue;

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
        if (other.tag == "Enemy")
        {
            enemyScript = other.GetComponent<EnemyBehaviour>();

            if (other.GetComponent<EnemyBehaviour>().hitByCurrent != hbSet) // If the hit object is an enemy : force and damage calculation goes here
            {
                enemyScript.timeReset = Time.time;
                enemyScript.health -= damage;
                enemyScript.hitByCurrent = hbSet;
                enemyScript.hitByLast = enemyScript.hitByCurrent;

                playerScript.currentMeter += meterValue;

                if (knockdown)
                {
                    enemyScript.Knockdown();
                }
                else
                {
                    other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    other.gameObject.GetComponent<Rigidbody>().AddForce(appliedForce * modForce * playerScript.facingSide, ForceMode.Impulse);
                }
            }
        }
    } // Called whenever something gets in the instantiated hurtbox
}
