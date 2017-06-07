using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EnemyAttackHurtboxing : NetworkBehaviour
{
    public Vector3 appliedForce = new Vector3(0, 1, 0);
    public float modForce = 5f;
    public float lifeSpan = 2f;
    private PlayerController playerScript;
    private bool timeCheck;
    private float timeWait;

    public int hbSet;
    public bool knockdown;
    public int damage;

    void Start()
    {
        //GameObject player = GameObject.Find("Player");
        GameObject player = GameObject.Find("Player_Network(Clone)");  //For the network
        playerScript = player.GetComponent<PlayerController>();
        timeWait = Time.time;
        //Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Enemy").GetComponent<Collider>(), GetComponent<Collider>());
    }

    void Update()
    {
        Destroy(gameObject, lifeSpan / 60);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerScript = other.GetComponent<PlayerController>();
            if (playerScript.hitByCurrent != hbSet)
            {
                if (playerScript.isBlocking)
                {
                    playerScript.blockPressure -= 1;
                }
                else
                {
                    playerScript.currentHP -= damage;
                    GameObject.Find("Health bar").transform.localScale = new Vector3((playerScript.currentHP / playerScript.totalHP), 1, 1);
                    playerScript.timeReset = Time.time;
                    playerScript.hitByCurrent = hbSet;
                    playerScript.hitByLast = playerScript.hitByCurrent;

                    if (knockdown)
                    {
                        other.GetComponent<EnemyBehaviour>().Knockdown();
                    }
                    else
                    {
                        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        other.gameObject.GetComponent<Rigidbody>().AddForce(appliedForce * modForce * playerScript.facingSide, ForceMode.Impulse);
                    }
                }    
            }
        }
    }
}// Called whenever something gets in the instantiated hurtbox
