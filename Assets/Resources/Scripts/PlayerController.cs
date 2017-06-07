using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    private bool paused;

    private float speed = 3;
    private float jumpSpeed = 4;
    private float doubleJumpSpeed = 5;

    private bool neutral, left, right, up, down;
    public bool lightAttack, heavyAttack, specialAttack, block;
    private bool jump;

    // Primary game physics interactions
    private Rigidbody rb;
    private Vector3 movement;

    public float facingSide { get; private set; }
    public float facingSideAir { get; private set; }

    // Player state
    private bool isGrounded = true;
    private bool jumpedMidair = false;
    public float totalHP = 10000;
    public float currentHP;
    public float totalMeter = 100;
    public float currentMeter;

    public bool canTakeAction; // = !(isAttacking || isStunned || attackBuffer.Count < 1)
    public bool isAttacking = false;
    public bool isStunned = false;
    public float stunFrames;
    public float stunTime;
    public float stunAtTime;

    public bool isBlocking = false;
    public float maxBlockPressure = 20;
    public float currentBlockPressure = 20;
    public float zbeb;
    private float timePressure;

    public int hitByCurrent = 0;
    public int hitByLast = 0;
    public float timeReset;
    
    // Attack logic
    private Vector3 hurtBoxFix = new Vector3(0, -0.5f, 0);
    private Vector3 tempFacingV3;

    public bool canCancel = false;
    public int currentMoveInCombo = 0;

    // Attacks
    public AttackDataEx groundLight1, groundLight2, groundLight3;
    public AttackDataEx jumpLight1, jumpLight2;

    public AttackDataEx groundHeavy1, groundHeavy2;
    public AttackDataEx jumpHeavy1, jumpHeavy2;

    public AttackDataEx groundSpecial1, groundSpecial2;
    public AttackDataEx jumpSpecial1, jumpSpecial2;

    public List<AttackDataEx> attackBuffer;

    //Animator
    protected Animator animator;
    private int _AnimatorAttack;
    public GameObject other;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tempFacingV3 = Vector3.right + Vector3.up;
        facingSide = 1f;
        attackBuffer = new List<AttackDataEx>();

        animator = other.GetComponent<Animator>();
        _AnimatorAttack = Animator.StringToHash("AnimatorAttack");
        animator.SetInteger(_AnimatorAttack, 0);

        currentHP = totalHP;
        currentMeter = 0;
        timePressure = Time.time;
    }
    void Update()
    {
        zbeb = currentBlockPressure / maxBlockPressure;
        /* Manages local player input */ 
        if (!isLocalPlayer)
        {
            return;
        }

        if (currentBlockPressure < 20)
        {
            if (Time.time >= timePressure + 1 / 2f)
            {
                timePressure = Time.time;
                currentBlockPressure = Mathf.Min(currentBlockPressure + 1, maxBlockPressure);
            }
        }

        if (hitByCurrent != 0 && hitByLast == hitByCurrent)
        {
            if (Time.time >= timeReset + 1 / 3f)
            {
                hitByCurrent = 0;
                hitByLast = 0;
            }
        } // Check for hitbox multiple hits

        PassThroughOthers();

        canTakeAction = !(isAttacking || isStunned || attackBuffer.Count > 1);

        isGrounded = rb.position.y < 1.095f;

        isStunned = stunFrames != 0;
        if (isStunned)
        {
            if (Time.time >= stunFrames / 60 + stunAtTime)
            {
                isStunned = false;
                stunFrames = 0;
            }
        }

        if (canTakeAction && !isBlocking) // Manages side inputs (TO BE REWORKED)
            SideInputCheck();

        if (isGrounded)
        {
            jumpedMidair = false;
            if (Input.GetButtonDown("light"))
            {
                Debug.Log("l attack");
                lightAttack = true;
            } // Light attacks inputs
            if (Input.GetButtonDown("heavy"))
            {
                Debug.Log("h attack");
                heavyAttack = true;
            } // Heavy attacks inputs
            if (Input.GetButtonDown("special"))
            {
                Debug.Log("s attack");
                specialAttack = true;
            } // Special attacks inputs
            if (Input.GetButton("block"))
            {
                Debug.Log("block");
                block = true;
            }
            else
            {
                block = false;
            }// Guard

            if (!isAttacking) // Horizontal movement and jumps
            {
                jumpedMidair = false;
                if (Input.GetButtonDown("jump"))
                {
                    jump = true;
                } // Jump


            } // Normal horizontal movement
        } // Grounded decision tree
        else
        {
            if (Input.GetButtonDown("light"))
            {
                Debug.Log("j.l attack");
                lightAttack = true;
            } // Normal attacks
            if (Input.GetButtonDown("heavy"))
            {
                Debug.Log("j.h attack");
                heavyAttack = true;
            } // Heavy attacks
            if (Input.GetButtonDown("special"))
            {
                Debug.Log("j.s attack");
                specialAttack = true;
            } // Special attacks
            if (Input.GetButtonDown("jump"))
            {
                jump = true;
            }
        } // Midair decision tree
    }
    void FixedUpdate()
    {
        if (canTakeAction)
        {
            if (jump && !isBlocking)
            {
                Debug.Log("jump");
                Jump();
            }

            if (isGrounded)
            {
                if (block)
                {
                    isBlocking = true;
                    return;
                }
                else
                {
                    isBlocking = false;
                }

                if (neutral) // Checked via SideInputCheck()
                {
                    rb.velocity = Vector3.zero;
                }
                else if (left || right)
                {
                    rb.velocity = new Vector3(1f * facingSide, 0, 0) * speed;
                }

                if (lightAttack)
                {
                    isAttacking = true;
                    lightAttack = false;

                    currentMoveInCombo = 11;
                    Attack(groundLight1);
                }
                else if (heavyAttack)
                {
                    isAttacking = true;
                    heavyAttack = false;

                    currentMoveInCombo = 21;
                    Attack(groundHeavy1);
                }
                else if (specialAttack)
                {
                    isAttacking = true;
                    specialAttack = false;

                    jumpedMidair = true;

                    currentMoveInCombo = 31;
                    Attack(groundSpecial1);
                }
            } // Grounded decision tree
            else
            {
                if (lightAttack)
                {
                    isAttacking = true;
                    lightAttack = false;

                    currentMoveInCombo = 111;
                    Attack(jumpLight1);
                }
                else if (heavyAttack)
                {
                    isAttacking = true;
                    heavyAttack = false;
                    currentMoveInCombo = 121;
                    Attack(jumpHeavy1);
                }
                else if (specialAttack)
                {
                    isAttacking = true;
                    specialAttack = false;
                    currentMoveInCombo = 131;

                    Attack(jumpSpecial1);
                }
            } // Midair decision tree
        }
        // Horizontal movement and combo starters
        else if (canCancel)
        {
            if (lightAttack)
            {
                isAttacking = true;
                lightAttack = false;
                switch (currentMoveInCombo)
                {
                    case 11:
                        Debug.Log(2);
                        currentMoveInCombo = 12;
                        Attack(groundLight2);
                        break;
                    case 12:
                        currentMoveInCombo = 13;
                        Attack(groundLight3);
                        break;
                }
            }
            else if (heavyAttack)
            {
                isAttacking = true;
                heavyAttack = false;
                switch (currentMoveInCombo)
                {
                    case 13:
                        currentMoveInCombo = 22;
                        Attack(groundHeavy2);
                        break;
                }

            }
            else if (specialAttack)
            {
                isAttacking = true;
                specialAttack = false;
                switch (currentMoveInCombo)
                {
                    case 13:
                        currentMoveInCombo = 31;
                        Attack(groundSpecial1);
                        break;
                }
            }
        } // Follow-ups
    }

    void SideInputCheck()
    {
        left = Input.GetAxis("Horizontal") < -0.20f;
        right = Input.GetAxis("Horizontal") > 0.20f;
        neutral = Input.GetAxis("Horizontal") >= -0.20f && Input.GetAxis("Horizontal") <= 0.20f;

        if (left)
        {
            if (isGrounded)
            {
                if (rb.transform.rotation.eulerAngles.y == 0 && isGrounded)
                    rb.GetComponent<Transform>().Rotate(Vector3.up * 180);
                tempFacingV3 = Vector3.left + Vector3.up;
            }
            facingSide = -1f;
        }
        else if (right)
        {
            if (isGrounded)
            {
                if (rb.transform.rotation.eulerAngles.y == 180 && isGrounded)
                    rb.GetComponent<Transform>().Rotate(Vector3.up * -180);
                tempFacingV3 = Vector3.right + Vector3.up;
            }
            facingSide = 1f;
        }

    } // Used to check whether the player is inputting any side

    void PassThroughOthers()     //So that players can pass through each other
    {
        GameObject[] otherPlayer;
        otherPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject other in otherPlayer)
            Physics.IgnoreCollision(other.GetComponentInChildren<Collider>(), GetComponentInChildren<Collider>());
    }

    void Jump()
    {
        if (isGrounded)
        {
            GroundedJumpDirection();
        }
        else
        {
            if (!jumpedMidair)
            {
                MidairJumpDirection();
                jumpedMidair = true;
            }
        }
        jump = false;
    }
    void GroundedJumpDirection()
    {
        if (neutral)
            movement = new Vector3(0f, 1.2f, 0.0f) * jumpSpeed;
        else
            movement = new Vector3(0.05f * facingSide, 1f, 0f) * jumpSpeed;
        rb.velocity = Vector3.zero;
        rb.AddForce(movement, ForceMode.Impulse);
    } // Used for grounded jump calculations (DO NOT TOUCH)
    void MidairJumpDirection()
    {
        if (neutral)
            movement = new Vector3(0f, 1.2f, 0.0f) * doubleJumpSpeed;
        else
            movement = new Vector3(0.45f * facingSide, 1f, 0f) * doubleJumpSpeed;

        rb.velocity = Vector3.zero;
        rb.AddForce(movement, ForceMode.Impulse);
    } // Used for midair jump calculations (DO NOT TOUCH)
    
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
        public int meterValue;

        public bool endsAerial;
        public bool knockdown;
        public bool isEnder;
    }
    IEnumerator AttackPattern(AttackDataEx data)
    {
        isAttacking = true;
        canCancel = true;
        yield return new WaitWhile(() => attackBuffer.Count != 0);
        Debug.Log(data.ID);
        attackBuffer.Add(data); // Add the current attack to the attackBuffer
        Debug.Log("Attacking with " + data.ID);
        
        yield return new WaitForSeconds(data.startupFrames / 60);
        animator.SetInteger(_AnimatorAttack, data.ID);
        

        for (int i = 0; i < data.hbData.Length; i++)
        {
            rb.AddForce(Vector3.Scale(data.hbData[i].playerForce, tempFacingV3), ForceMode.Impulse);
            string path = "Prefabs/Hurtboxes/";
            string hurt = (data.hbData[i].hurtBox).name;

            if (hurt.StartsWith("Light"))
                path += "Light/";
            else if (hurt.StartsWith("Heavy"))
                path += "Heavy/";
            else if (hurt.StartsWith("Special"))
                path += "Special/";

            CmdHurt(path + hurt,
                gameObject.transform.position + Vector3.Scale(data.hbData[i].hurtBox.transform.position, tempFacingV3),
                gameObject.transform.rotation,
                data.damage,
                data.meterValue,
                data.knockdown,
                data.ID);
            
            if (data.endsAerial && jumpedMidair == false)
                jumpedMidair = true;

            yield return new WaitForSeconds(data.hbData[i].activeFrames / 60);
        }

        attackBuffer.Remove(data);
        animator.SetInteger(_AnimatorAttack, 0);
        yield return new WaitForSeconds(data.recoveryFrames / 60);

        if (attackBuffer.Count == 0)
        {
            currentMoveInCombo = 0;
            canCancel = false;
            isAttacking = false;
        }
    }

    [Command]
    void CmdHurt(string hurt, Vector3 pos, Quaternion rot, int damage, int meterValue, bool knockdown, int ID)
    {
        var att = (GameObject)Instantiate(Resources.Load(hurt, typeof(GameObject)), pos, rot);
        att.GetComponent<PlayerAttackHurtboxing>().knockdown = knockdown;
        att.GetComponent<PlayerAttackHurtboxing>().damage = damage;
        att.GetComponent<PlayerAttackHurtboxing>().hbSet = ID;
        att.transform.parent = GameObject.FindWithTag("Player").transform;
        NetworkServer.Spawn(att);
    }
}
