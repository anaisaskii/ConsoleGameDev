using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    private EnemySpawner enemySpawner;

    public override void Die()
    {
        /*
        if (enemySpawner != null)
        {
            enemySpawner.RemoveEnemy(gameObject);
        }

        Destroy(gameObject);
        */

        Destroy(gameObject);
    }
}
