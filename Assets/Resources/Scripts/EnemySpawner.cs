using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public int numberOfEnemies;

    public override void OnStartServer()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            var enemy = (GameObject)Instantiate(enemyPrefab, gameObject.transform.position, gameObject.transform.rotation);
            NetworkServer.Spawn(enemy);
        }
    }
}