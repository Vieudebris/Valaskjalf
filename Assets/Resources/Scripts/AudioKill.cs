using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioKill : MonoBehaviour {

    void Start()
    {
        Destroy(gameObject, 2);
    }
}
