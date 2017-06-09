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

    public short hbSet;
    public bool knockdown;
    public int damage;
    public float stun;

    public AudioSource[] soundOnHit;
    private System.Random randomSound;

    void Start()
    {
        //GameObject player = GameObject.Find("Player");
        GameObject player = GameObject.Find("Player_Network(Clone)");  //For the network
        playerScript = player.GetComponent<PlayerController>();
        timeWait = Time.time;
        //Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Enemy").GetComponent<Collider>(), GetComponent<Collider>());
        soundOnHit = GameObject.Find("Main Camera/audio/onhit").GetComponents<AudioSource>();
        randomSound = new System.Random();
    }

    void Update()
    {
        Destroy(gameObject, lifeSpan / 60);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && ForSolo.serverID == other.GetComponent<PlayerController>().netId)
        {
            if (playerScript.hitByCurrent != hbSet)
            {
                if (playerScript.isBlocking)
                {
                    playerScript.hitByCurrent = hbSet;
                    playerScript.hitByLast = playerScript.hitByCurrent;
                    GameObject.Find("Main Camera/audio/").GetComponent<AudioSource>().Play();
                    playerScript.currentBlockPressure -= 1;
                }
                else
                {
                    soundOnHit[randomSound.Next(4, 9)].Play();
                    //Instantiate(sound, GameObject.Find("Main Camera/audio").transform);
                    playerScript.TakeDamage(damage);

                    playerScript.stunAtTime = Time.time;
                    playerScript.stunFrames = stun;

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

                playerScript.updateUI = true;
            }
        }
    }
}// Called whenever something gets in the instantiated hurtbox
