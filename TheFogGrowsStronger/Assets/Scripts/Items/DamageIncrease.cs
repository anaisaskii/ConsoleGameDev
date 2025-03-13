using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIncrease : Item
{
    public override void ApplyEffect(GameObject player)
    {
        base.ApplyEffect(player);

        //grab player controller script from playerrr
        PlayerController stats = player.GetComponent<PlayerController>();

        if (stats != null)
        {
            stats.projectileDamage *= powerScaling; //multiply so it adds up (or you could do plus for specific items..?)
            Debug.Log(stats.projectileDamage);
        }
    }
}
