using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    GameObject paused;
    public GameObject pauseMenu;

    public bool optiBool = true;

    
    void Start()
    {
        Time.timeScale = 1;
        paused = GameObject.Find("Paused");
        pauseMenu = GameObject.Find("PauseMenu");
        paused.SetActive(false);
        pauseMenu.SetActive(false);
    }
    
    void Update()
    {
        if ((pauseMenu.activeSelf || paused.activeSelf) && optiBool)
        {
            GameObject.Find("Main Camera").GetComponent<AudioSource>().volume = 0.1f;
            optiBool = false;
        }
        
        if (!pauseMenu.activeSelf && !paused.activeSelf && !optiBool)
        {
            GameObject.Find("Main Camera").GetComponent<AudioSource>().volume = 0.5f;
            optiBool = true;
        }

        if (Input.GetButtonDown("Submit") && !pauseMenu.activeSelf)
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                paused.SetActive(true);
            }
            else if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                paused.SetActive(false);
            }
        }

        else if (Input.GetButtonDown("Cancel") && !paused.activeSelf)
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
            }
            else if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
            }
        }
    }
}
