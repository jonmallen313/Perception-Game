using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public Transform player;

    public GameObject lootPrefab;

    public float triggerDistance = 10f;

    public int maxEnemies = 5;
    public float spawnDelay = 1f; 

    public enum SpawnMode { AllAtOnce, DelayBetween, WaitUntilDead }
    public SpawnMode spawnMode = SpawnMode.AllAtOnce;

    private bool hasSpawned = false;


    private List<GameObject> activeEnemies = new List<GameObject>();

    void Update()
    {
        if (!hasSpawned && Vector3.Distance(player.position, transform.position) <= triggerDistance)
        {
            hasSpawned = true;

            StartCoroutine(SpawnEnemies());


        }



    }

    IEnumerator SpawnEnemies()
    {
        switch (spawnMode)
        {
            case SpawnMode.AllAtOnce:
                yield return new WaitForSeconds(spawnDelay);
                for (int i = 0; i < maxEnemies; i++)
                    SpawnOne();
                break;

            case SpawnMode.DelayBetween:
                for (int i = 0; i < maxEnemies; i++)
                {
                    SpawnOne();
                    yield return new WaitForSeconds(spawnDelay);
                }
                break;

            case SpawnMode.WaitUntilDead:
                for (int i = 0; i < maxEnemies; i++)
                {
                    SpawnOne();
                    
                    yield return new WaitUntil(() => activeEnemies[activeEnemies.Count - 1] == null);
                }
                break;


        }
    }

    void SpawnOne()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);


        EnemyCombat combatScript = enemy.GetComponent<EnemyCombat>();
        combatScript.Loot = lootPrefab;


        activeEnemies.Add(enemy);


    }
}
