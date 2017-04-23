using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    private float speed = 3;
    private float jumpSpeed = 4;
    private float doubleJumpSpeed = 5;

    private bool neutral, left, right, up, down;
    public bool lightAttack, heavyAttack, specialAttack;
    private bool jump;

    // Primary game physics interactions
    private Rigidbody rb;
    private Vector3 movement;

    public float facingSide { get; private set; }
    public float facingSideAir { get; private set; }

    // Player state
    private bool isGrounded = true;
    public bool isAttacking = false;
    private bool jumpedMidair = false;
    private int playerHealth = 1000;
    private bool isStunned = false;

    // Attack logic
    private Vector3 hurtBoxFix = new Vector3(0, -0.5f, 0);
    private float moveEndTime;
    private float comboTimeThreshold = 0.5f;

    private Vector3 tempFacingV3;

    public bool attackCancel = false;
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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tempFacingV3 = Vector3.right + Vector3.up;
        facingSide = 1f;
        attackBuffer = new List<AttackDataEx>();
    }
    void Update()
    {
        /* Manages local player input */ 
        if (!isLocalPlayer)
        {
            return;
        }

        isGrounded = rb.position.y < 1.095f;

        if (attackBuffer.Count == 0)
        {
            isAttacking = false;
        }

        if (!isAttacking) // Manages side inputs (TO BE REWORKED)
            SideInputCheck();

        if (isGrounded)
        {
            jumpedMidair = false;
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("l attack");
                lightAttack = true;
            } // Light attacks inputs
            if (Input.GetButtonDown("Fire2"))
            {
                Debug.Log("h attack");
                heavyAttack = true;
            } // Heavy attacks inputs
            if (Input.GetButtonDown("Fire3"))
            {
                Debug.Log("s attack");
                specialAttack = true;
            } // Special attacks inputs

            if (!isAttacking) // Horizontal movement and jumps
            {
                jumpedMidair = false;
                if (Input.GetButtonDown("Jump"))
                {
                    jump = true;
                } // Jump


            } // Normal horizontal movement
        } // Grounded decision tree
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("j.l attack");
                lightAttack = true;
            } // Normal attacks
            if (Input.GetButtonDown("Fire2"))
            {
                Debug.Log("j.h attack");
                heavyAttack = true;
                attackCancel = true;
            } // Heavy attacks
            else
            {
                attackCancel = false;
            }
            if (Input.GetButtonDown("Fire3"))
            {
                Debug.Log("j.s attack");
                specialAttack = true;
                attackCancel = true;
            } // Special attacks
            else
            {
                attackCancel = false;
            }
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
            }
        } // Midair decision tree
    }
    void FixedUpdate()
    {
        if (!isAttacking)
        {
            if (jump)
            {
                Debug.Log("jump");
                Jump();
            }

            if (isGrounded)
            {
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
            Debug.Log(2);
            if (lightAttack)
            {
                Debug.Log(1);
                attackCancel = true;
                isAttacking = true;
                lightAttack = false;
                switch (currentMoveInCombo)
                {
                    case 11:
                        Debug.Log(0);
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
                attackCancel = true;
                heavyAttack = false;
                switch (currentMoveInCombo)
                {
                    case 11:
                        currentMoveInCombo = 21;
                        Attack(groundHeavy1);
                        break;
                    case 13:
                        currentMoveInCombo = 22;
                        Attack(groundHeavy2);
                        break;
                }

            }
            else if (specialAttack)
            {
                isAttacking = true;
                attackCancel = true;
                specialAttack = false;
                switch (currentMoveInCombo)
                {
                    case 13:
                        currentMoveInCombo = 31;
                        Attack(groundSpecial1);
                        break;
                    case 21:
                        currentMoveInCombo = 32;
                        Attack(groundSpecial2);
                        break;
                    case 31:
                        currentMoveInCombo = 132;
                        Attack(jumpSpecial2);
                        break;
                }
            }
        } // Follow-ups

        if (jump)
        {
            Debug.Log("jump");
            Jump();
        }
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
        rb.velocity = Vector3.zero;
        StartCoroutine(AttackPattern(attack));
    }

    public void CancelCalculations()
    {
        
    }


    public override void OnStartLocalPlayer()   //Colors the local player in blue (needs to be changed)
    {
        GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
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
    IEnumerator AttackPattern(AttackDataEx data)
    {
        while (attackBuffer.Count > 1)
        { }
        attackBuffer.Add(data); // Add the current attack to the attackBuffer
        Debug.Log("Attacking with " + data.ID);
        yield return new WaitForSeconds(data.startupFrames / 60);
        canCancel = false;
        for (int i = 0; i < data.hbData.Length; i++)
        {
            rb.AddForce(Vector3.Scale(data.hbData[i].playerForce, tempFacingV3), ForceMode.Impulse);
            string path = "Prefabs/Hurtboxes/";
            string hurt = ((data.hbData[i].hurtBox)).name;

            if (hurt.StartsWith("Light"))
                path += "Light/";
            else if (hurt.StartsWith("Heavy"))
                path += "Heavy/";
            else if (hurt.StartsWith("Special"))
                path += "Special/";

            CmdHurt(path + hurt, gameObject.transform.position + Vector3.Scale(data.hbData[i].hurtBox.transform.position, tempFacingV3), gameObject.transform.rotation);
            
            if (data.endsAerial && jumpedMidair == false)
                jumpedMidair = true;

            yield return new WaitForSeconds(data.hbData[i].activeFrames / 60);
        }

        canCancel = true;
        attackBuffer.Remove(data);
        yield return new WaitForSeconds(data.recoveryFrames / 60);
        if (attackBuffer.Count == 0)
        {
            currentMoveInCombo = 0;
            attackCancel = false;
            canCancel = false;
            isAttacking = false;
        }
    }

    [Command]
    void CmdHurt (string hurt, Vector3 pos, Quaternion rot)
    {
        var att = (GameObject)Instantiate(Resources.Load(hurt, typeof(GameObject)), pos, rot);
        NetworkServer.Spawn(att);
    }
}
