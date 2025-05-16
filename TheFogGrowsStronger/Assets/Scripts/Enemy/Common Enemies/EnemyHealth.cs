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

    //Despawns enemy upon death and updates active enemies in scene
    public override void Die()
    {
        enemySpawner.RemoveEnemy(this.gameObject);
        Destroy(this.gameObject);
    }
}
