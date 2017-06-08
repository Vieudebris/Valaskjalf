using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLocker : MonoBehaviour {

    public CameraFollow cameraScript;
    public bool turnOff;
    public GameObject[] barriers;

    private bool hasBeenActivated = false;

	// Use this for initialization
	void Start () {

        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        barriers = new GameObject[2];
        barriers[0] = GameObject.Find("left");
        barriers[1] = GameObject.Find("right");

        for (int i = 0; i < GetComponentsInChildren<MeshRenderer>().Length; i++)
        {
            //GetComponentsInChildren<MeshRenderer>()[i].enabled = false;
        }

        barriers[0].SetActive(false);
        barriers[1].SetActive(false);
	}

    private void Update()
    {
        if (turnOff && barriers[1].activeSelf)
        {
            Destroy(barriers[1]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !hasBeenActivated)
        {
            barriers[0].SetActive(true);
            barriers[1].SetActive(true);

            cameraScript.maxXAndY = new Vector2(gameObject.transform.position.x, 10);
            cameraScript.minXAndY = new Vector2(gameObject.transform.position.x, 0);

            hasBeenActivated = true;
        }
    }
}
