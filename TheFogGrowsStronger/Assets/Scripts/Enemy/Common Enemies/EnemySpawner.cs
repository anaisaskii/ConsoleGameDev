using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform player;
    public GameObject enemyPrefab;
    public int maxEnemies = 10; //max enemies in scene
    public float spawnRadius = 20f; //average radius
    public float minSpawnDistance = 10f; //the minimum distance enemies can spawn around the player
    public float enemyRadius = 1.5f; // how much space each enemy needs
    public float spawnCooldown = 2f;

    //Patrol waypoints
    public Transform[] waypoints;
    public Transform[] bossWaypoints;
    public Transform[] boss2Waypoints;

    //active enemies in scene
    private List<GameObject> activeEnemies = new List<GameObject>();
    private float spawnTimer = 0f; //for cooldown

    void Update()
    {
        spawnTimer += Time.deltaTime; //reset cooldown

        //if cooldown is low enough and less than 10 enemies in scene spawn a new enemy
        if (spawnTimer >= spawnCooldown && activeEnemies.Count < maxEnemies)
        {
            Vector3 spawnPos;
            if (TryGetValidSpawnPosition(out spawnPos))
            {
                GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                activeEnemies.Add(newEnemy);

                activeEnemies.RemoveAll(e => e == null);
                spawnTimer = 0f;
            }
        }
    }

    //check if the spawn position is valid
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

    //remove the enemy from the list when it's killed
    public void RemoveEnemy(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }
}
