using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour {

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

        barriers[0].SetActive(false);
        barriers[1].SetActive(false);
	}

    private void Update()
    {
        if (turnOff)
        {
            cameraScript.maxXAndY = new Vector2(10000, 10);
            cameraScript.minXAndY = new Vector2(-10000, 0);
            Destroy(barriers[1]);
            turnOff = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !hasBeenActivated)
        {
            StartCoroutine(SmoothCamLock());

            barriers[0].SetActive(true);
            barriers[1].SetActive(true);

            hasBeenActivated = true;
        }
    }

    IEnumerator SmoothCamLock()
    {
        yield return new WaitForSeconds(2);
        cameraScript.maxXAndY = new Vector2(gameObject.transform.position.x, 10);
        cameraScript.minXAndY = new Vector2(gameObject.transform.position.x, 0);
    }
}
