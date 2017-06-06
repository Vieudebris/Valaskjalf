﻿using System;
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

    private Vector3 tempFacingV3;

    // Attacks
    public AttackDataEx attack1, attack2, attack3;
    public List<AttackDataEx> attackBuffer;

    // Enemy state
    private bool isGrounded = true;
    private bool isAttacking = false;
    private bool jumpedMidair = false;
    public int health = 2000;
    private bool isStunned = false;
    private bool canTakeAction;

    public int hitByCurrent = 0;
    public int hitByLast = 0;
    public float timeReset;

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
        if (hitByCurrent != 0 && hitByLast == hitByCurrent)
        {
            timeReset = Time.time;
            if (Time.time >= timeReset + 1/3f)
            {
                hitByCurrent = 0;
                hitByLast = 0;
            }
        } // Check for hitbox multiplicity of hits

        canTakeAction = !(isAttacking || isStunned || attackBuffer.Count > 1);
        if (GameObject.FindGameObjectWithTag("Player"))
            player = FindClosestPlayer();
        else
            player = enemy;

        dist = player.position.x - enemy.position.x;
        nav.stoppingDistance = 2;
        SideCheck();
        PassThroughOthers();

        int att = rand_att.Next(1, 4);

        if (canTakeAction)
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

        if (health <= 0)
        {
            Kill();
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

    void Kill()
    {

    }

    public void Knockdown() // Knocks down if needed, called by PlayerAttackHurtboxing
    {

    }

    void PassThroughOthers()    //So that enemies can pass through each others
    {
        GameObject[] otherEnemies;
        otherEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in otherEnemies)
            Physics.IgnoreCollision(enemy.GetComponent<Collider>(), GetComponent<Collider>());
    }

    //Creates Enemy's attacks
    void Attack(AttackDataEx attack)
    {
        StartCoroutine(AttackPattern(attack));
    }

    [System.Serializable]
    public class AttackData
    {
        public GameObject hurtBox;
        public float activeFrames;
        public Vector3 playerForce;
        public bool lastHb;
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

    IEnumerator AttackPattern(AttackDataEx data)
    {
        attackBuffer.Add(data); // Add the current attack to the attackBuffer
        yield return new WaitForSeconds(data.startupFrames / 60);
        for (int i = 0; i < data.hbData.Length; i++)
        {
            rb.AddForce(Vector3.Scale(data.hbData[i].playerForce, tempFacingV3), ForceMode.Impulse);

            string path = "Prefabs/Hurtboxes/Enemy1";
            string hurt = (data.hbData[i].hurtBox).name;

            CmdHurt(path + hurt,
                gameObject.transform.position + Vector3.Scale(data.hbData[i].hurtBox.transform.position, tempFacingV3),
                gameObject.transform.rotation,
                data.damage,
                data.knockdown,
                data.ID);

            yield return new WaitForSeconds(data.hbData[i].activeFrames / 60);
        }
        
        attackBuffer.Remove(data);
        yield return new WaitForSeconds(data.recoveryFrames / 60);

        if (attackBuffer.Count == 0)
            isAttacking = false;

    } // Still doesn't manage network, check CmdHurt arguments

    [Command]
    void CmdHurt(string hurt, Vector3 pos, Quaternion rot, int damage, bool knockdown, int ID)
    {
        var att = (GameObject)Instantiate(Resources.Load(hurt, typeof(GameObject)), pos, rot);
        att.GetComponent<EnemyAttackHurtboxing>().knockdown = knockdown;
        att.GetComponent<EnemyAttackHurtboxing>().damage = damage;
        att.GetComponent<EnemyAttackHurtboxing>().hbSet = ID;
        att.transform.parent = GameObject.FindWithTag("Player1").transform;
        NetworkServer.Spawn(att);
    }
}
