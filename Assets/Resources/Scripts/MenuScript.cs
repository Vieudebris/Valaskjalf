using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class MenuScript : MonoBehaviour {

    Button butt; 
    public string newsc; 

	void Start ()
    {
        butt = GetComponent<Button>();
        butt.onClick.AddListener(LoadScene); 
	}
	public void LoadScene ()
    {
        SceneManager.LoadScene(newsc, LoadSceneMode.Single); 
    }

	// Update is called once per frame
	void Update () {
		
	}
}
