using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class EnemyMovement : MonoBehaviour

{

    private Transform player;                       // Reference to the player's position.

    private Transform ennemy;

    private Vector3[] points = new Vector3[2];      // Reference to where the ennemy go if it doesn't see the player

    private int destPoint = 0;

    private UnityEngine.AI.NavMeshAgent nav;

    public float view;                              // Distance minimum necessary for the ennemy to see the player and follow it

    public float range_of_pat;                      //Defines how far the ennemy goes when it doesn't see the player



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

}