using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 2;
    public float jumpSpeed = 5;
    public float doubleJumpSpeed = 5;

    public GameObject AA, AB, AC, BA, BB, BC;

    private bool neutral, left, right, up, down, jump, lightAttack, heavyAttack;

    // Primary game physics interactions
    private Rigidbody rb;
    private Vector3 movement;

    public float facingSide { get; private set; }
    private float distToGround;

    // Secondary game physics interactions
    private bool canMove = true;
    private bool isMidair = false;
    private bool isAtRest = true;
    private bool jumpedMidair = false;


    // Attack logic
    public Vector3 hurtBoxFix = new Vector3(0, -0.5f, 0);
    private bool attackCancel = false;
    private int hbChecker = 0;
    private float moveEndTime;
    private float comboTimeThreshold = 0.8f;

    private float tempFacing;
    private Vector3 tempFacingV3;

    private int currentMoveInRevolverAction = 0;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tempFacingV3 = Vector3.right;
    }
    void Start () {
        
    }
    void Update ()
    {
        distToGround = rb.GetComponentInChildren<Collider>().bounds.extents.y;

        isMidair = rb.position.y > 1.095f;
        isMidair = Physics.Raycast(transform.position, Vector3.down, distToGround + 1f);

        lightAttack = Input.GetButtonDown("Fire1");
        heavyAttack = Input.GetButtonDown("Fire2");
        attackCancel = heavyAttack;
        jump = Input.GetButtonDown("Jump");

        SideInputCheck();

        // Managing Light Attacks WIP
        /*if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("light attack");
            lightAttack = true;
        }*/


        // Managing Heavy Attacks WIP
        /*if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("heavy attack");
            heavyAttack = true;
            attackCancel = true;
        }
        else
            attackCancel = false;*/

        // Managing Jump WIP
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (!isMidair && canMove)
        {
            jumpedMidair = false;
            if (neutral)
                rb.velocity = Vector3.zero;
            else
                rb.velocity = new Vector3(1f * facingSide, 0, 0) * speed;
        }

    }
    void FixedUpdate()
    {
        if (jump)
        {
            Debug.Log("jump");
            Jump();
        }

        if (lightAttack)
        {
            canMove = false;
            tempFacing = facingSide;
            tempFacingV3 *= tempFacing;
            NeutralLight();
        }
    }

    void SideInputCheck()
    {
        left = Input.GetAxis("Horizontal") < -0.20f;
        right = Input.GetAxis("Horizontal") > 0.20f;
        neutral = Input.GetAxis("Horizontal") >= -0.20f && Input.GetAxis("Horizontal") <= 0.20f;
        if (left)
        {
            facingSide = -1f;
        }
        else if (right)
        {
            facingSide = 1f;
        }
    } // Used to check whether the player is inputting any side

    void Jump()
    {    
        if (!isMidair)
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
    } // Used for grounded jump calculations
    void MidairJumpDirection()
    {
        if (neutral)
            movement = new Vector3(0f, 1.2f, 0.0f) * doubleJumpSpeed;
        else
            movement = new Vector3(0.45f * facingSide, 1f, 0f) * doubleJumpSpeed;
            
        rb.velocity = Vector3.zero;
        rb.AddForce(movement, ForceMode.Impulse);
    } // Used for midair jump calculations

    void NeutralLight()
    {
        StartCoroutine(LightA1());
        StartCoroutine(LightA2());

        lightAttack = false;
        canMove = true;
        attackCancel = false;
    }
    IEnumerator LightA1()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(new Vector3(10f * facingSide, 0.5f, 0f), ForceMode.Impulse);
        yield return new WaitForSeconds(5/60f);
        if (hbChecker == 0)
        {
            Instantiate(AA, gameObject.transform.position + Vector3.Scale(AA.transform.position, tempFacingV3), gameObject.transform.rotation);
            hbChecker++;
        }
        CancelCalculations();
        yield return new WaitForSeconds(2/60f);
        if (hbChecker == 1)
        {
            Instantiate(AB, gameObject.transform.position + Vector3.Scale(AB.transform.position, tempFacingV3), gameObject.transform.rotation);
            hbChecker++;
        }
        CancelCalculations();
        yield return new WaitForSeconds(1/60f);
        if (hbChecker == 2)
        {
            Instantiate(AC, gameObject.transform.position + Vector3.Scale(AC.transform.position, tempFacingV3), gameObject.transform.rotation);
            hbChecker++;
        }
        hbChecker = 0;
        moveEndTime = Time.time;
    }
    IEnumerator LightA2()
    {
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
    } // Neutral light attack, it works smh
    public void CancelCalculations()
    {
        if (attackCancel)
            hbChecker = 0;
    }

    IEnumerator HurtboxGeneration(GameObject prefab, float waitTime, int comboCounter)
    {
        yield return new WaitForSeconds(waitTime);
        if (hbChecker == comboCounter)
        {
            Instantiate(prefab, gameObject.transform.position + Vector3.Scale(prefab.transform.position, tempFacingV3), gameObject.transform.rotation);
            hbChecker++;
        }
    }

    [System.Serializable]
    public class AttackData
    {
        public GameObject hurtBox;
        public float delay;
        public int hbSet;
    }
    public AttackData[] L;
    IEnumerator Attack(AttackData[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            Instantiate(data[i].hurtBox, gameObject.transform.position + Vector3.Scale(data[i].hurtBox.transform.position, tempFacingV3), gameObject.transform.rotation);
            CancelCalculations();
            yield return new WaitForSeconds(data[i].delay);
        }
    }
}
