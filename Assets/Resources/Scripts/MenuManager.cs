using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public GameObject display1;
    public GameObject display2;
    public GameObject displayMult;
    public GameObject displayMatch;
    public GameObject msg1;
    public GameObject displayJoin;
    public GameObject msg2;
    public GameObject msg3;

    // Use this for initialization
    void Start ()
    {
        display1 = GameObject.Find("Display1");
        display2 = GameObject.Find("Display2");
        display2.SetActive(false);
        displayMult = GameObject.Find("DisplayMult");
        displayMult.SetActive(false);
        displayMatch = GameObject.Find("DisplayMatch");
        displayMatch.SetActive(false);
        msg1 = GameObject.Find("msg1");
        msg1.SetActive(false);
        displayJoin = GameObject.Find("DisplayJoin");
        displayJoin.SetActive(false);
        msg2 = GameObject.Find("msg2");
        msg2.SetActive(false);
        msg3 = GameObject.Find("msg3");
        msg3.SetActive(false);
    }
}
