using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed = 3;
    private float jumpSpeed = 5;
    private float doubleJumpSpeed = 5;

    private bool neutral, left, right, up, down;
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
    private bool jumpedMidair = false;
    private bool isDashing = false;
    private bool doubleTap = false;

    // Attack logic
    private Vector3 hurtBoxFix = new Vector3(0, -0.5f, 0);
    private bool attackCancel = false;
    private int hbChecker = 0;
    private float moveEndTime;
    private float comboTimeThreshold = 0.8f;

    private Vector3 tempFacingV3;

    private int currentMoveInRevolverAction = 0;

    // Attacks
    public AttackDataEx groundLight1, groundLight2, groundLight3;
    public AttackDataEx jumpLight1, jumpLight2;

    public AttackDataEx groundHeavy1, groundHeavy2;
    public AttackDataEx jumpHeavy1, jumpHeavy2;

    public AttackDataEx groundSpecial1, groundSpecial2;
    public AttackDataEx jumpSpecial1, jumpSpecial2;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tempFacingV3 = Vector3.right + Vector3.up;
    }
    void Update()
    {
        /*distToGround = rb.GetComponentInChildren<Collider>().bounds.extents.y;
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);*/

        isGrounded = rb.position.y < 1.095f;

        if (!isAttacking) // Manages side inputs (TO BE REWORKED)
        {
            SideInputCheck();
        }

        if (isGrounded) // Grounded decision tree
        {
            jumpedMidair = false;
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("l attack");
                lightAttack = true;
            } // Light attacks
            if (Input.GetButtonDown("Fire2"))
            {
                Debug.Log("h attack");
                heavyAttack = true;
                attackCancel = true;
            } // Heavy attacks
            else
            {
                attackCancel = false;
            }
            if (Input.GetButtonDown("Fire3"))
            {
                Debug.Log("s attack");
                specialAttack = true;
                attackCancel = true;
            } // Special attacks
            else
            {
                attackCancel = false;
            }

            if (!isAttacking) // Horizontal movement and jumps
            {
                jumpedMidair = false;
                if (Input.GetButtonDown("Jump"))
                {
                    jump = true;
                } // Jump

                
            } // Normal horizontal movement
        }
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
                    Attack(groundLight1);
                }
                else if (heavyAttack)
                {
                    isAttacking = true;
                    heavyAttack = false;
                    Attack(groundHeavy1);
                }
                else if (specialAttack)
                {
                    isAttacking = true;
                    specialAttack = false;

                    jumpedMidair = true;

                    Attack(groundSpecial1);
                }
            }
            else
            {
                if (lightAttack)
                {
                    isAttacking = true;
                    lightAttack = false;
                    Attack(jumpLight1);
                }

                if (heavyAttack)
                {
                    isAttacking = true;
                    heavyAttack = false;
                    Attack(jumpHeavy1);
                }

                if (specialAttack)
                {
                    isAttacking = true;
                    specialAttack = false;
                    Attack(jumpSpecial1);
                }
            }
        }

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
            movement = new Vector3(0.45f * facingSide, 1f, 0f) * jumpSpeed;
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
        yield return new WaitForSeconds(data.startupFrames/60);
        for (int i = 0; i < data.hbData.Length; i++)
        {
            rb.AddForce(data.hbData[i].playerForce * facingSide, ForceMode.Impulse);

            Instantiate(data.hbData[i].hurtBox, gameObject.transform.position + Vector3.Scale(data.hbData[i].hurtBox.transform.position, tempFacingV3), gameObject.transform.rotation);
            CancelCalculations();
            yield return new WaitForSeconds(data.hbData[i].activeFrames/60);
        }
        yield return new WaitForSeconds(data.recoveryFramesOnMiss/60);
        isAttacking = false;
    }
}
