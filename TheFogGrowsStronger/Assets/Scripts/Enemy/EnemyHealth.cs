using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    public EnemySpawner enemySpawner;

    public override void Die()
    {
        /*
        if (enemySpawner != null)
        {
            enemySpawner.RemoveEnemy(gameObject);
        }

        Destroy(gameObject);
        */

        Debug.Log("EnemyHealth script - Die()");
    }
}
