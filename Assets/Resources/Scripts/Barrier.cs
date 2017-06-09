using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Barrier : NetworkBehaviour {

    public CameraFollow cameraScript;
    public bool turnOff;
    public GameObject[] barriers;
    public GameObject spawner;
    public EnemySpawner script;

    private bool hasBeenActivated = false;

	// Use this for initialization
	void Start () {

        barriers = new GameObject[2];
        barriers[0] = GameObject.Find("left");
        barriers[1] = GameObject.Find("right");

        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        script = spawner.GetComponent<EnemySpawner>();

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

            script.enable = true;
        }

        turnOff = true;
    }

    IEnumerator SmoothCamLock()
    {
        yield return new WaitForSeconds(2);
        cameraScript.maxXAndY = new Vector2(gameObject.transform.position.x, 10);
        cameraScript.minXAndY = new Vector2(gameObject.transform.position.x, 0);
    }
}
