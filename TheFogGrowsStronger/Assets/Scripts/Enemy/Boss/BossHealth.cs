using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : Health
{
    BossBT bossBT;

    void Start()
    {
        bossBT = this.GetComponent<BossBT>();
    }

    // destroy prefab when the boss dies
    public override void Die()
    {
        Destroy(gameObject);
    }
}
