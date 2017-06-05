using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class MenuScript : MonoBehaviour {

    Button butt; 
    MenuManager menuManager;
    PauseMenu pauseMenuclass;

    public bool isExit, isSolo, isMulti, isOptions;

    public bool isBoxTraining, isNiflheim, isReturn;

    public bool isRestartLevel, isReturnToMenu, isExitGame, isResume;

	void Start ()
    {
        butt = GetComponent<Button>();
        butt.onClick.AddListener(LoadScene);

        if (SceneManager.GetActiveScene().name == "Menu")
            menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();

        else
            pauseMenuclass = GameObject.Find("PauseManager").GetComponent<PauseMenu>();
        
    }

	public void LoadScene ()
    {
        if (isExit || isExitGame)
            Application.Quit();

        /* Main Menu */
        if (isSolo || isMulti)
        {
            menuManager.display1.SetActive(false);
            menuManager.display2.SetActive(true);
        }

        if (isReturn)
        {
            menuManager.display1.SetActive(true);
            menuManager.display2.SetActive(false);
        }
        
        if (isBoxTraining)
            SceneManager.LoadScene("The Box", LoadSceneMode.Single);

        if (isNiflheim)
            SceneManager.LoadScene("Niflheim", LoadSceneMode.Single);

        /*Pause Menu */
        if (isRestartLevel)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);

        if (isReturnToMenu)
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);

        if (isResume)
        {
            Time.timeScale = 1;
            pauseMenuclass.pauseMenu.SetActive(false);
        }


    }
}
