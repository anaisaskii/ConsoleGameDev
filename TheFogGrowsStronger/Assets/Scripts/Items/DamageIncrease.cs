using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIncrease : Item
{
    private TextMeshProUGUI gemText;
    //Static so that it doesn't keep resetting itself when spawned in
    private static int gems = 0;

    public override void ApplyEffect(GameObject player)
    {
        base.ApplyEffect(player);

        gemText = GameObject.Find("GemText").GetComponent<TextMeshProUGUI>();

        //grab player controller script from playerrr
        PlayerController stats = player.GetComponent<PlayerController>();

        if (stats != null)
        {
            gems += 1;
            stats.projectileDamage *= powerScaling;
            gemText.text = "x" + gems.ToString();
        }
    }
}
