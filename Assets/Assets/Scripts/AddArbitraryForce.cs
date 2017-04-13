using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddArbitraryForce : MonoBehaviour {

    private Rigidbody rb;
	// Update is called once per frame
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

	void Update () {
		if (Input.GetButtonDown("Fire2"))
        {
            rb.velocity = new Vector3(-10, 2, 0);
        }
	}
}
