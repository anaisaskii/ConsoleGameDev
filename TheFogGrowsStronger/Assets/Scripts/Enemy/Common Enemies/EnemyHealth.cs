using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    private EnemySpawner enemySpawner;

    private void Start()
    {
        enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
    }

    public override void Die()
    {
        enemySpawner.RemoveEnemy(this.gameObject);
        Destroy(this.gameObject);
    }
}
