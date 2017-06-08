using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public GameObject display1;
    public GameObject display2a;
    public GameObject display2b;
    public GameObject display2c;
    public GameObject displayMult;
    public GameObject displayMatch;
    public GameObject msg1;
    public GameObject displayJoin;
    public GameObject msg2;
    public GameObject msg3;
    public GameObject msg4;
    public GameObject msg5;
    public GameObject msg6;
    public GameObject msg0;
    public GameObject returnJoined;

    // Use this for initialization
    void Start ()
    {
        display1 = GameObject.Find("Display1");
        display2a = GameObject.Find("Display2a");
        display2a.SetActive(false);
        display2b = GameObject.Find("Display2b");
        display2b.SetActive(false);
        display2c = GameObject.Find("Display2c");
        display2c.SetActive(false);
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
        msg4 = GameObject.Find("msg4");
        msg4.SetActive(false);
        msg5 = GameObject.Find("msg5");
        msg5.SetActive(false);
        msg6 = GameObject.Find("msg6");
        msg6.SetActive(false);
        msg0 = GameObject.Find("msg0");
        msg0.SetActive(false);
        returnJoined = GameObject.Find("ReturnJoined");
        returnJoined.SetActive(false);
    }
}
