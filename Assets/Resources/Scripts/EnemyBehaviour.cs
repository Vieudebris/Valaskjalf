using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyBehaviour : NetworkBehaviour
{
    System.Random rand_att = new System.Random(); //To pick randomly an attack

    private Transform player;       // Reference to the player's position.
    private Transform enemy;        // Reference to the enemy's position.

    // For enemy's movements
    private UnityEngine.AI.NavMeshAgent nav;
    float dist;

    // Game physics interactions
    private Rigidbody rb;
    private Vector3 movement;

    public float facingSide { get; private set; }
    public float facingSideAir { get; private set; }
    private bool isGrounded = true;
    private bool isAttacking = false;

    private Vector3 tempFacingV3;

    // Attacks
    public AttackDataEx attack1, attack2, attack3;
    public List<AttackDataEx> attackBuffer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tempFacingV3 = Vector3.right + Vector3.up;
        enemy = GetComponent<Transform>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        nav.autoBraking = false;
        attackBuffer = new List<AttackDataEx>();
    }


    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
            player = FindClosestPlayer();
        else
            player = enemy;

        dist = player.position.x - enemy.position.x;
        nav.stoppingDistance = 2;
        SideCheck();

        int att = rand_att.Next(1, 4);

        if (!isAttacking)
        {
            if (Mathf.Abs(dist) < 2)
            {
                isAttacking = true;

                switch (att)
                {
                    case 1:
                        Attack(attack1);
                        break;
                    case 2:
                        Attack(attack2);
                        break;
                    case 3:
                        Attack(attack3);
                        break;
                }
            }
            else
                nav.SetDestination(player.position);
        }
    }

    void SideCheck() //Checks if it goes to the left or to the right
    {
        if (dist < 0)
        {
            if (isGrounded)
            {
                if (rb.transform.rotation.eulerAngles.y == 0 && isGrounded)
                    rb.GetComponent<Transform>().Rotate(Vector3.up * 180);
                tempFacingV3 = Vector3.left + Vector3.up;
            }
            facingSide = -1f;
        }
        else
        {
            if (isGrounded)
            {
                if (rb.transform.rotation.eulerAngles.y == 180 && isGrounded)
                    rb.GetComponent<Transform>().Rotate(Vector3.up * -180);
                tempFacingV3 = Vector3.right + Vector3.up;
            }
            facingSide = 1f;
        }
    }

    
    Transform FindClosestPlayer()       //Returns the position of the closest player (for multiplayer)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest.transform;
    }

    //Creates Enemy's attacks
    void Attack(AttackDataEx attack)
    {
        rb.velocity = Vector3.zero;
        StartCoroutine(AttackPattern(attack));
    }

    [System.Serializable]
    public class AttackData
    {
        public GameObject hurtBox;
        public float activeFrames;
        public int hbSet;
        public Vector3 playerForce;
    }

    [System.Serializable]
    public class AttackDataEx
    {
        public int ID;

        public AttackData[] hbData;
        public float startupFrames;
        public float recoveryFrames;

        public ParticleSystem SFX;
        public Animation anim;
        public AudioClip sound;

        public float stunFrames;
        public int damage;

        public bool endsAerial;
        public bool knockdown;
        public bool isEnder;
    }
    /*IEnumerator AttackPattern(AttackDataEx data)
    {
        yield return new WaitForSeconds(data.startupFrames / 60);
        for (int i = 0; i < data.hbData.Length; i++)
        {
            rb.AddForce(Vector3.Scale(data.hbData[i].playerForce, tempFacingV3), ForceMode.Impulse);

            Instantiate(data.hbData[i].hurtBox, gameObject.transform.position + Vector3.Scale(data.hbData[i].hurtBox.transform.position, tempFacingV3), gameObject.transform.rotation);
            yield return new WaitForSeconds(data.hbData[i].activeFrames / 60);
        }
        yield return new WaitForSeconds(data.recoveryFramesOnMiss / 60 + 1);
        isAttacking = false;
    }*/

    IEnumerator AttackPattern(AttackDataEx data)
    {
        while (attackBuffer.Count > 1)
        { }
        attackBuffer.Add(data); // Add the current attack to the attackBuffer
        Debug.Log("Attacking with " + data.ID);
        yield return new WaitForSeconds(data.startupFrames / 60);
        for (int i = 0; i < data.hbData.Length; i++)
        {
            rb.AddForce(Vector3.Scale(data.hbData[i].playerForce, tempFacingV3), ForceMode.Impulse);

            Instantiate(data.hbData[i].hurtBox, gameObject.transform.position + Vector3.Scale(data.hbData[i].hurtBox.transform.position, tempFacingV3), gameObject.transform.rotation);
            
            yield return new WaitForSeconds(data.hbData[i].activeFrames / 60);
        }
        
        attackBuffer.Remove(data);
        yield return new WaitForSeconds(data.recoveryFrames / 60);

        if (attackBuffer.Count == 0)
            isAttacking = false;

    }
}
