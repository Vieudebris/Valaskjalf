using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public int numberOfEnemies;
    public int DelayBetweenSpawn;

    public override void OnStartServer()
    {
            StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            var enemy = (GameObject)Instantiate(enemyPrefab, gameObject.transform.position, gameObject.transform.rotation);
            NetworkServer.Spawn(enemy);
            yield return new WaitForSeconds(DelayBetweenSpawn);
        }
    }
}