﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ForSolo : MonoBehaviour {

    bool solo = MenuScript.solo;
    public static NetworkInstanceId serverID;
    
	void Start ()
    {
        if (solo)
        {
            NetworkManagerHUD net = GameObject.Find("NetworkManager").GetComponent<NetworkManagerHUD>();
            NetworkManager.singleton.StartHost();
            net.showGUI = false;
        }

        NetworkManager.singleton.autoCreatePlayer = true;
    }
}
