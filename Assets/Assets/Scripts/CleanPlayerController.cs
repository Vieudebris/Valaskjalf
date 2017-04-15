using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanPlayerController : MonoBehaviour {

    private float speed = 2;
    private float jumpSpeed = 5;
    private float doubleJumpSpeed = 5;

    public bool lightAttack, heavyAttack, specialAttack;
    public bool jump;
    public bool up, down, left, right, neutral;

    private Rigidbody rb;

    private float facingSide;
    private Vector3 tempFacingV3;

    public bool isGrounded = true;
    public bool jumpedMidair;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tempFacingV3 = Vector3.right + Vector3.up;
    }

    void Start()
    {
        
    }

    void Update()
    {
        isGrounded = rb.position.y < 1.095f;
        SideInputCheck();

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
            } // Heavy attacks
            if (Input.GetButtonDown("Fire3"))
            {
                Debug.Log("s attack");
                specialAttack = true;
            } // Special attacks

            if (isGrounded) // Horizontal movement and jumps
            {
                jumpedMidair = false;
                if (Input.GetButtonDown("Jump"))
                {
                    jump = true;
                } // Jump
                
            } // Normal horizontal movement
        }    
    }
    void FixedUpdate()
    {
        if (neutral) // Checked via SideInputCheck()
        { rb.velocity = Vector3.zero; }
        else
        { rb.velocity = new Vector3(1f * facingSide, 0, 0) * speed; }
    }
    void SideInputCheck()

    {
        left = Input.GetAxis("Horizontal") < -0.20f;
        right = Input.GetAxis("Horizontal") > 0.20f;
        neutral = Input.GetAxis("Horizontal") >= -0.20f && Input.GetAxis("Horizontal") <= 0.20f;

        if (left)
        {
            if (rb.transform.rotation.eulerAngles.y == 0 && isGrounded)
                rb.GetComponent<Transform>().Rotate(Vector3.up * 180);
            facingSide = -1f;
            tempFacingV3 = Vector3.left + Vector3.up;
        }
        else if (right)
        {
            if (rb.transform.rotation.eulerAngles.y == 180 && isGrounded)
                rb.GetComponent<Transform>().Rotate(Vector3.up * -180);
            facingSide = 1f;
            tempFacingV3 = Vector3.right + Vector3.up;
        }
    } // Used to check whether the player is inputting any side
}
