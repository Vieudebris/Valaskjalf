using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public GameObject hazard;
    public Vector3 spawnValues;

    public int spawnWait;

    private int clock;

    void Start()
    {
       
    }

    IEnumerator Template()
    {
        yield return new WaitForSeconds(0);
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }
}
