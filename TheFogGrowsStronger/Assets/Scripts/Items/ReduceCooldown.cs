using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReduceCooldown : Item
{
    private TextMeshProUGUI pillText;
    //Static so that it doesn't keep resetting itself when spawned in
    private static int pills = 0;

    public override void ApplyEffect(GameObject player)
    {
        base.ApplyEffect(player);

        pillText = GameObject.Find("PillText").GetComponent<TextMeshProUGUI>();

        //grab player controller script from playerrr
        PlayerController stats = player.GetComponent<PlayerController>();

        if (stats != null)
        {
            pills += 1;
            stats.secondaryAttackCooldown /= 1.05f;
            pillText.text = "x" + pills.ToString();
        }
    }
}
