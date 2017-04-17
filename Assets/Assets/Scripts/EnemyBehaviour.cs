using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {

    private Transform player;                       // Reference to the player's position.
    private Transform ennemy;

    // For enemy's movements
    private Vector3[] points = new Vector3[2];      // Reference to where the ennemy go if it doesn't see the player
    private int destPoint = 0;
    private UnityEngine.AI.NavMeshAgent nav;
    public float view;                              // Distance minimum necessary for the ennemy to see the player and follow it
    public float range_of_pat;                      //Defines how far the ennemy goes when it doesn't see the player


    /*Copied from PlayerController (until line 63) */

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
     /*copy ends here */
    


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ennemy = GetComponent<Transform>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        nav.autoBraking = false;
        points[0] = new Vector3(ennemy.position.x - range_of_pat, 0, 0);
        points[1] = new Vector3(ennemy.position.x + range_of_pat, 0, 0);
    }


    void GotoNextPoint()
    {
        // Set the agent to go to the currently selected destination.
        nav.destination = points[destPoint];

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }
    

    void Update()
    {
        float dist = Mathf.Abs(player.position.x - ennemy.position.x);

        if (dist < view)
        {
            nav.SetDestination(player.position);
            nav.stoppingDistance = 2;
        }
        else
            GotoNextPoint();
    }
    
    //copied from PlayerController
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
            rb.AddForce(data.hbData[i].playerForce * facingSide, ForceMode.Impulse);

            Instantiate(data.hbData[i].hurtBox, gameObject.transform.position + Vector3.Scale(data.hbData[i].hurtBox.transform.position, tempFacingV3), gameObject.transform.rotation);
            CancelCalculations();
            yield return new WaitForSeconds(data.hbData[i].activeFrames / 60);
        }
        yield return new WaitForSeconds(data.recoveryFramesOnMiss / 60);
        isAttacking = false;
    }
}
