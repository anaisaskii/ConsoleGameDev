using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform player;
    public GameObject enemyPrefab;
    public int maxEnemies = 10;
    public float spawnRadius = 20f;
    public float minSpawnDistance = 10f;
    public float enemyRadius = 1.5f; // how much space each enemy needs
    public float spawnCooldown = 2f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float spawnTimer = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Current active enemies: " + activeEnemies.Count);
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnCooldown && activeEnemies.Count < maxEnemies)
        {
            Vector3 spawnPos;
            if (TryGetValidSpawnPosition(out spawnPos))
            {
                GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                //newEnemy.GetComponent<Enemy>().spawner = this;
                activeEnemies.Add(newEnemy);

                // Optional: clean up destroyed enemies
                activeEnemies.RemoveAll(e => e == null);
                spawnTimer = 0f;
            }
        }
    }

    bool TryGetValidSpawnPosition(out Vector3 result)
    {
        int attempts = 10;
        while (attempts-- > 0)
        {
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = 0;
            Vector3 candidatePos = player.position + randomDir.normalized * Random.Range(minSpawnDistance, spawnRadius);

            if (IsPositionClear(candidatePos))
            {
                result = candidatePos;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    bool IsPositionClear(Vector3 position)
    {
        // Check if the spot is free from other enemies
        foreach (var enemy in activeEnemies)
        {
            if (enemy == null) continue;
            if (Vector3.Distance(position, enemy.transform.position) < enemyRadius * 2f)
            {
                return false;
            }
        }
        return true;
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }
}
