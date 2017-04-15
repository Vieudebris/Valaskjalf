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
    private float distToGround;

    // Secondary game physics interactions
    private bool isGrounded = true;
    private bool isAttacking = false;
    private bool jumpedMidair = false;
    private bool isDashing = false;
    private float dashTime;
    private bool doubleTapD = false;

    // Attack logic
    private Vector3 hurtBoxFix = new Vector3(0, -0.5f, 0);
    private bool attackCancel = false;
    private int hbChecker = 0;
    private float moveEndTime;
    private float comboTimeThreshold = 0.8f;

    private Vector3 tempFacingV3;

    private int currentMoveInRevolverAction = 0;

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
                heavyAttack = false;
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

            if (isGrounded && !isAttacking) // Horizontal movement and jumps
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
        if (isGrounded && !isAttacking)
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
                NeutralLight();
            }
        }

        if (jump)
        {
            Debug.Log("jump");
            Jump();
        }

        if (lightAttack)
        {
            isAttacking = true;
            NeutralLight();
        }

        if (heavyAttack)
        {
            heavyAttack = false;
        }

        if (specialAttack)
        {
            specialAttack = false;
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
    void NeutralLight()
    {
        lightAttack = false;
        rb.velocity = Vector3.zero;
        rb.AddForce(new Vector3(1f * facingSide, 0f, 0f), ForceMode.Impulse);
        StartCoroutine(Attack(gL1));
        CancelCalculations();
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
        public float delay;
        public int hbSet;
        public Vector3 playerForce;
    }
    public AttackData[] gL1;
    IEnumerator Attack(AttackData[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i].playerForce.x *= facingSide;
            rb.AddForce(data[i].playerForce, ForceMode.Impulse);

            Instantiate(data[i].hurtBox, gameObject.transform.position + Vector3.Scale(data[i].hurtBox.transform.position, tempFacingV3), gameObject.transform.rotation);
            CancelCalculations();
            yield return new WaitForSeconds(data[i].delay);
        }
        isAttacking = false;
    }
}
/*    IEnumerator LightA1()
    {
        isAttacking = true;
        rb.velocity = Vector3.zero;
        rb.AddForce(new Vector3(10f * facingSide, 0.5f, 0f), ForceMode.Impulse);
        yield return new WaitForSeconds(5 / 60f);
        if (hbChecker == 0)
        {
            Instantiate(AA, gameObject.transform.position + Vector3.Scale(AA.transform.position, tempFacingV3), gameObject.transform.rotation); // First hurtbox
            hbChecker++;
        }
        yield return new WaitForSeconds(2 / 60f);
        if (hbChecker == 1)
        {
            Instantiate(AB, gameObject.transform.position + Vector3.Scale(AB.transform.position, tempFacingV3), gameObject.transform.rotation); // First hurtbox
            hbChecker++;
        }
        yield return new WaitForSeconds(1 / 60f);
        if (hbChecker == 2)
        {
            Instantiate(AC, gameObject.transform.position + Vector3.Scale(AC.transform.position, tempFacingV3), gameObject.transform.rotation); // First hurtbox
            hbChecker++;
        }
        moveEndTime = Time.time;
        hbChecker = 0;
        yield break;
    } // obsolete, use IEnumerator Attack()
    IEnumerator LightA2()
    {
        isAttacking = true;
        rb.velocity = Vector3.zero;
        rb.AddForce(new Vector3(1f * facingSide, 0f, 0f), ForceMode.Impulse);
        yield return new WaitForSeconds(5 / 60f);
        if (hbChecker == 0)
        {
            Instantiate(BA, gameObject.transform.position + Vector3.Scale(BA.transform.position, tempFacingV3), gameObject.transform.rotation);
            hbChecker++;
        }
        CancelCalculations();
        yield return new WaitForSeconds(2 / 60f);
        if (hbChecker == 1)
        {
            Instantiate(BB, gameObject.transform.position + Vector3.Scale(BB.transform.position, tempFacingV3), gameObject.transform.rotation);
            hbChecker++;
        }
        CancelCalculations();
        yield return new WaitForSeconds(1 / 60f);
        if (hbChecker == 2)
        {
            Instantiate(BC, gameObject.transform.position + Vector3.Scale(BC.transform.position, tempFacingV3), gameObject.transform.rotation);
            hbChecker++;
        }
        hbChecker = 0;
        attackCancel = false;
    } // obsolete, use IEnumerator Attack()

    IEnumerator HurtboxGeneration(GameObject prefab, float waitTime, int hbCounter, Vector3 dash)
    {
        yield return new WaitForSeconds(waitTime);
        if (hbChecker == hbCounter)
        {
            Instantiate(prefab, gameObject.transform.position + Vector3.Scale(prefab.transform.position, tempFacingV3), gameObject.transform.rotation);
            hbChecker++;
        }
    }*/
