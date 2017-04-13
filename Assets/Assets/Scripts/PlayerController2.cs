/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour {

    public float speed = 2;
    public float jumpSpeed = 5;
    public float doubleJumpSpeed = 5;

    public GameObject AA, AB, AC, BA, BB, BC;

    private bool neutral, left, right, up, down, jump, lightAttack, heavyAttack;

    // Game physics interactions

    private Rigidbody rb;

    private bool canHMove;



    // Physical states

    private bool isGrounded;
    private bool isMidair;
    private bool hasJumped2;
    private bool isAttacking;

    // Attack logic



    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();

	}
	
	// Update is called once per frame
	void Update () {
		
        isGrounded = rb.position.y < 1.095f;
        hasJumped2 = isGrounded;

        if (Input.GetButtonDown("Jump"))
        {
            if ()
        }



    }

    void FixedUpdate()
    {
        
    }

    void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

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
}*/
