using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;

    public List<GameObject> enemyList;
    public List<GameObject> enemyListBuffer;

    public int numberOfEnemies;
    public int DelayBetweenSpawn;

    public bool enable = false;

    public GameObject relatedBarrier;
    private Barrier barrierScript;
    private bool hasSpawned;

    public int allNull;

    public override void OnStartServer()
    {
        if (gameObject.tag == "Spawner0")
            relatedBarrier = GameObject.FindGameObjectWithTag("trigger0");

        if (gameObject.tag == "Spawner1")
            relatedBarrier = GameObject.FindGameObjectWithTag("trigger1");

        if (gameObject.tag == "Spawner2")
            relatedBarrier = GameObject.FindGameObjectWithTag("trigger2");

        if (gameObject.tag == "Spawner3")
            relatedBarrier = GameObject.FindGameObjectWithTag("trigger3");

        if (gameObject.tag == "Spawner4")
            relatedBarrier = GameObject.FindGameObjectWithTag("trigger4");

        barrierScript = relatedBarrier.GetComponentInChildren<Barrier>();
        enemyList = new List<GameObject>();
    }

    IEnumerator SpawnEnemy()
    {
        bool x = true;
        Vector2 v;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (x)
            {
                v = new Vector2(gameObject.transform.position.x + 10, 5);
                x = false;
            }
            else
            {
                v = new Vector2(gameObject.transform.position.x - 10, 5);
                x = true;
            }
                
            var enemy = (GameObject)Instantiate(enemyPrefab, v, gameObject.transform.rotation);
            enemyList.Add(enemy);
            NetworkServer.Spawn(enemy);
            yield return new WaitForSeconds(DelayBetweenSpawn);
            hasSpawned = true;
        }

        enemyListBuffer = enemyList;
    }

    private void Update()
    {
        if (enable)
        {
            StartCoroutine(SpawnEnemy());
            enable = false;
            return;
        }

        if (hasSpawned)
        {
            foreach (GameObject enemy in enemyList)
            {
                if (enemy != null)
                {
                    return;
                }
            }
            barrierScript.turnOff = true;
        }
    }
}