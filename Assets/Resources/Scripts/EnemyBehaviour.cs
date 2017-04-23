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

    /*Copied from PlayerController (until line 63) */

    private bool lightAttack, heavyAttack, specialAttack;
    private bool jump;

    // Primary game physics interactions
    private Rigidbody rb;
    private Vector3 movement;

    public float facingSide { get; private set; }
    public float facingSideAir { get; private set; }

    // Secondary game physics interactions
    private bool isGrounded = true;
    private bool isAttacking = false;

    // Attack logic
    private Vector3 hurtBoxFix = new Vector3(0, -0.5f, 0);
    private bool attackCancel = false;
    private int hbChecker = 0;
    private float moveEndTime;
    private float comboTimeThreshold = 0.8f;

    private Vector3 tempFacingV3;

    // Attacks
    public AttackDataEx attack1, attack2, attack3;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tempFacingV3 = Vector3.right + Vector3.up;
        enemy = GetComponent<Transform>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        nav.autoBraking = false;
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

        if (Mathf.Abs(dist) < 2 && !isAttacking)
        {
            isAttacking = true;

            switch (att)
            {
                case 1:
                    Debug.Log("Enemy_test attack1");
                    Attack(attack1);
                    break;
                case 2:
                    Debug.Log("Enemy_test attack2");
                    Attack(attack2);
                    break;
                case 3:
                    Debug.Log("Enemy_test attack3");
                    Attack(attack3);
                    break;
            }
        }

        else
            nav.SetDestination(player.position);
    }

    void SideCheck() 
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
        lightAttack = false;
        rb.velocity = Vector3.zero;
        StartCoroutine(AttackPattern(attack));
    }

    public void CancelCalculations()
    {
        if (attackCancel)
            hbChecker = 0;
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
        public AttackData[] hbData;
        public float startupFrames;
        public float recoveryFramesOnMiss;
        public float recoveryFramesOnHit;

        public ParticleSystem SFX;
        public Animation anim;
        public AudioClip sound;

        public float stunFrames;
        public bool knockdown;
        public int damage;
    }

    IEnumerator AttackPattern(AttackDataEx data)
    {
        yield return new WaitForSeconds(data.startupFrames / 60);
        for (int i = 0; i < data.hbData.Length; i++)
        {
            rb.AddForce(Vector3.Scale(data.hbData[i].playerForce, tempFacingV3), ForceMode.Impulse);

            Instantiate(data.hbData[i].hurtBox, gameObject.transform.position + Vector3.Scale(data.hbData[i].hurtBox.transform.position, tempFacingV3), gameObject.transform.rotation);
            yield return new WaitForSeconds(data.hbData[i].activeFrames / 60);
        }
        yield return new WaitForSeconds(data.recoveryFramesOnMiss / 60);
        isAttacking = false;
    }
}
