using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceCooldown : Item
{
    public override void ApplyEffect(GameObject player)
    {
        base.ApplyEffect(player);

        //grab player controller script from playerrr
        PlayerController stats = player.GetComponent<PlayerController>();

        if (stats != null)
        {
            stats.secondaryAttackCooldown *= 1.05f;
            Debug.Log(stats.secondaryAttackCooldown);
        }
    }
}
