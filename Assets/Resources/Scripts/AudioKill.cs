using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioKill : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 4);
	}
}
