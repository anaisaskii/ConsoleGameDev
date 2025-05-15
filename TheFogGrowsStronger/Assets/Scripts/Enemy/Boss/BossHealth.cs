using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : Health
{
    BossBT bossBT;
    // Start is called before the first frame update
    void Start()
    {
        bossBT = this.GetComponent<BossBT>();
    }

    public override void TakeDamage(float damage)
    {
        //base.TakeDamage(damage);    
        bossBT.OnPlayerAttack();

        base.TakeDamage(damage);
    }
}
