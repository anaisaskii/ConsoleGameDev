using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraBullet : Item
{
    public override void ApplyEffect(GameObject player)
    {
        base.ApplyEffect(player);

        //grab player controller script from playerrr
        PlayerController stats = player.GetComponent<PlayerController>();

        if (stats != null)
        {
            //can have this affect walking and sprinting seperately for different items
            //in different functions or something
            //this just affects them both for now
            stats.fireRate *= 1.15f;
        }
    }
}
