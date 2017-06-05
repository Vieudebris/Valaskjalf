using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public GameObject display1;
    public GameObject display2;

    // Use this for initialization
    void Start ()
    {
        display1 = GameObject.Find("Display1");
        display2 = GameObject.Find("Display2");
        display2.SetActive(false);
    }
}
