using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class MenuScript : MonoBehaviour {

    Button butt; 
    MenuManager menuManager;
    public bool isExit, isSolo, isMulti, isOptions;

    public bool isBoxTraining, isNiflheim, isReturn;

	void Start ()
    {
        butt = GetComponent<Button>();
        butt.onClick.AddListener(LoadScene);
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();

    }

	public void LoadScene ()
    {
        if (isExit)
            Application.Quit();

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
            SceneManager.LoadScene("Box Training", LoadSceneMode.Single);

        if (isNiflheim)
            SceneManager.LoadScene("Niflheim", LoadSceneMode.Single);
    }
}
