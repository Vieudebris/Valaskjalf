using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomName : MonoBehaviour
{

    static public string roomName = "";

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        Font vik = (Font)Resources.Load("Textures/VIKING-N", typeof(Font));
        style.font = vik;
        style.fontSize = 15;
        roomName = GUI.TextField(new Rect(Screen.width / 2 - 90, Screen.height / 2, 247, 30), roomName, 17, style);
    }
}
