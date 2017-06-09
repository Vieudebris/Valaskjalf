using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Barrier : NetworkBehaviour {

    public CameraFollow cameraScript;
    public bool turnOff;
    public GameObject spawner;
    public EnemySpawner script;
    public GameObject left, right;

    private bool hasBeenActivated = false;

	// Use this for initialization
	void Start () {

        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        script = spawner.GetComponent<EnemySpawner>();

        left.SetActive(false);
        right.SetActive(false);
	}

    private void Update()
    {
        if (turnOff)
        {
            cameraScript.maxXAndY = new Vector2(10000, 10);
            cameraScript.minXAndY = new Vector2(-10000, 0);
            Destroy(right);
            turnOff = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !hasBeenActivated)
        {
            if (!(SceneManager.GetActiveScene().name == "Midgard"))
                StartCoroutine(SmoothCamLock());

            left.SetActive(true);
            right.SetActive(true);

            hasBeenActivated = true;

            script.enable = true;
        }
    }

    IEnumerator SmoothCamLock()
    {
        yield return new WaitForSeconds(2);
        cameraScript.maxXAndY = new Vector2(gameObject.transform.position.x, 10);
        cameraScript.minXAndY = new Vector2(gameObject.transform.position.x, 0);
    }
}
