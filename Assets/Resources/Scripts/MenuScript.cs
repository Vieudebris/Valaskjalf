using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MenuScript : MonoBehaviour
{

    Button butt;
    MenuManager menuManager;
    PauseMenu pauseMenuclass;
    NetworkManagerHUD net;
    string matchN;
    static List<MatchInfoSnapshot> matchesAvailable;

    public bool isExit, isSolo, isMulti, isOptions;

    public bool isMidgard, isNiflheim, isNidavellir, isReturn;

    public bool isRestartLevel, isReturnToMenu, isExitGame, isResume;

    public bool isCreateMatch, isJoinMatch, isCreate, isMatch1;

    public bool isNexta, isNextb, isPreviousb, isPreviousc, isLocked; 

    public static bool solo = false;

    void Start()
    {
        butt = GetComponent<Button>();
        butt.onClick.AddListener(LoadScene);
        net = GameObject.Find("NetworkManager").GetComponent<NetworkManagerHUD>();

        if (SceneManager.GetActiveScene().name == "Menu")
            menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();

        else
            pauseMenuclass = GameObject.Find("PauseManager").GetComponent<PauseMenu>();

    }

    public void LoadScene()
    {
        if (isExit || isExitGame)
            Application.Quit();

        /* Main Menu */
        if (isSolo)
        {
            menuManager.display1.SetActive(false);
            menuManager.display2a.SetActive(true);
            solo = true;
        }

        if (isMulti)
        {
            menuManager.display1.SetActive(false);
            menuManager.displayMult.SetActive(true);
            solo = false;
        }

        if (isLocked)
        {
            menuManager.msg0.SetActive(true);
        }

        if (isNexta)
        {
            menuManager.display2a.SetActive(false);
            menuManager.display2b.SetActive(true);
            menuManager.msg0.SetActive(false);
        }

        if (isNextb)
        {
            menuManager.display2b.SetActive(false);
            menuManager.display2c.SetActive(true);
            menuManager.msg0.SetActive(false);
        }

        if (isPreviousb)
        {
            menuManager.display2b.SetActive(false);
            menuManager.display2a.SetActive(true);
            menuManager.msg0.SetActive(false);
        }

        if (isPreviousc)
        {
            menuManager.display2c.SetActive(false);
            menuManager.display2b.SetActive(true);
            menuManager.msg0.SetActive(false);
        }

        if (isReturn)
        {
            if (menuManager.display2a.activeSelf)
            {
                menuManager.display1.SetActive(true);
                menuManager.display2a.SetActive(false);
            }

            if (menuManager.display2b.activeSelf)
            {
                menuManager.display1.SetActive(true);
                menuManager.display2b.SetActive(false);
            }

            if (menuManager.display2c.activeSelf)
            {
                menuManager.display1.SetActive(true);
                menuManager.display2c.SetActive(false);
            }

            if (menuManager.displayMult.activeSelf)
            {
                menuManager.display1.SetActive(true);
                menuManager.displayMult.SetActive(false);
            }

            if (menuManager.displayMatch.activeSelf)
            {
                menuManager.displayMult.SetActive(true);
                menuManager.displayMatch.SetActive(false);
                menuManager.msg1.SetActive(false);
            }

            if (menuManager.displayJoin.activeSelf)
            {
                menuManager.displayMult.SetActive(true);
                menuManager.displayJoin.SetActive(false);
                menuManager.msg2.SetActive(false);
                menuManager.msg3.SetActive(false);
            }

            if (menuManager.msg4.activeSelf && menuManager.msg6.activeSelf)
            {
                menuManager.msg4.SetActive(false);
                menuManager.msg6.SetActive(false);
                menuManager.displayJoin.SetActive(true);
            }

            menuManager.msg0.SetActive(false);
            NetworkManager.singleton.StopMatchMaker();

            if (solo == true)
                solo = false;
        }

        if (isCreateMatch)
        {
            menuManager.displayMatch.SetActive(true);
            menuManager.displayMult.SetActive(false);
        }

        if (isCreate)
        {
            NetworkManager.singleton.StartMatchMaker();
            NetworkManager.singleton.matchMaker.CreateMatch(RoomName.roomName, 2, true, "", "", "", 0, 0, OnInternetMatchCreate);
            menuManager.msg1.SetActive(true);

            StartCoroutine(isConnected());
        }

        if (isJoinMatch)
        {
            menuManager.displayJoin.SetActive(true);
            menuManager.displayMult.SetActive(false);
            
            NetworkManager.singleton.StartMatchMaker();
            menuManager.msg3.SetActive(true);
            NetworkManager.singleton.matchMaker.ListMatches(0, 10, "", true, 0, 0, OnInternetMatchList);
        }

        if (isMatch1 && GameObject.Find("Match1").GetComponentInChildren<Text>().text != null)
        {
            NetworkManager.singleton.matchMaker.JoinMatch(matchesAvailable[0].networkId, "", "", "", 0, 0, OnJoinInternetMatch);
        }

        if (isMidgard)
            NetworkManager.singleton.ServerChangeScene("Midgard");

        if (isNiflheim)
            NetworkManager.singleton.ServerChangeScene("Niflheim");

        if (isNidavellir)
            NetworkManager.singleton.ServerChangeScene("Nidavellir");

        /*Pause Menu */
        if (isRestartLevel)
        {
            NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);

            if (solo)
                NetworkManager.singleton.StopHost();
        }

        if (isReturnToMenu)
        {
            NetworkManager.singleton.ServerChangeScene("Menu");
            net.showGUI = false;

            if (solo)
                NetworkManager.singleton.StopHost();
        }

        if (isResume)
        {
            Time.timeScale = 1;
            pauseMenuclass.pauseMenu.SetActive(false);
        }
    }

    /* Functions needed for matchmaking */
    private void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Create match succeeded");

            MatchInfo hostInfo = matchInfo;
            NetworkServer.Listen(hostInfo, 9000);

            NetworkManager.singleton.StartHost(hostInfo);
        }
        else
        {
            Debug.LogError("Create match failed");
        }
    }

    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        menuManager.msg3.SetActive(false);

        if (success)
        {
            if (matches.Count != 0)
            {
                Debug.LogAssertion("A list of matches was returned");
                matchN = matches[0].name;
                matchesAvailable = matches;
                GameObject.Find("Match1").GetComponentInChildren<Text>().text = matchN;
            }
            else
            {
                Debug.LogAssertion("No matches in requested room!");
            }
        }
        else
        {
            menuManager.msg2.SetActive(true);
            Debug.LogAssertion("Couldn't connect to match maker");
        }
    }

    private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Able to join a match");
            menuManager.msg4.SetActive(true);
            MatchInfo hostInfo = matchInfo;
            NetworkManager.singleton.StartClient(hostInfo);
            menuManager.displayJoin.SetActive(false);
            menuManager.msg6.SetActive(true);
            menuManager.returnJoined.SetActive(true);
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }

    IEnumerator isConnected()
    {
        do { yield return new WaitForSeconds(5); }
        while (NetworkServer.connections.Count < 2);

        if (NetworkServer.connections.Count == 2)
        {
            menuManager.displayMatch.SetActive(false);
            menuManager.display2a.SetActive(true);
            menuManager.msg1.SetActive(false);
            menuManager.msg5.SetActive(true);
        }
    }
}


