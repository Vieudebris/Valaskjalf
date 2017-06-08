using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;

    public GameObject[] enemyArray;

    public int numberOfEnemies1;
    public int numberOfEnemies2;
    public int DelayBetweenSpawn;

    public bool enable1;
    public bool enable2;

    public GameObject relatedBarrier;
    private Barrier barrierScript;

    public override void OnStartServer()
    {
        relatedBarrier = GameObject.Find("barrier");
        barrierScript = relatedBarrier.GetComponentInChildren<Barrier>();
    }

    IEnumerator SpawnEnemy1()
    {
        enemyArray = new GameObject[numberOfEnemies1 + numberOfEnemies2];

        for (int i = 0; i < numberOfEnemies1; i++)
        {
            var enemy = (GameObject)Instantiate(enemyPrefab, new Vector2(gameObject.transform.position.x + 10, 5), gameObject.transform.rotation);
            enemyArray[i] = enemy;
            NetworkServer.Spawn(enemy);
            yield return new WaitForSeconds(DelayBetweenSpawn);
        }
    }
    IEnumerator SpawnEnemy2()
    {
        for (int i = 0; i < numberOfEnemies2; i++)
        {
            var enemy = (GameObject)Instantiate(enemyPrefab, new Vector2(gameObject.transform.position.x - 10, 5), gameObject.transform.rotation);
            enemyArray[i] = enemy;
            NetworkServer.Spawn(enemy);
            yield return new WaitForSeconds(DelayBetweenSpawn);
        }
    }

    private void Update()
    {
        if (enable1 || enable2)
        {
            StartCoroutine(SpawnEnemy1());
            StartCoroutine(SpawnEnemy2());
            enable2 = false;
            enable1 = false;
            return;
        }

        foreach (GameObject enemy in enemyArray)
        {
            if (enemy != null)
                return;
        }
        barrierScript.turnOff = true;
    }
}