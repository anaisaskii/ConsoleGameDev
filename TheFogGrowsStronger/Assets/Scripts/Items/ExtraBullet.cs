using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExtraBullet : Item
{
    private TextMeshProUGUI magText;
    //Static so that it doesn't keep resetting itself when spawned in
    private static int mags = 0;
    public override void ApplyEffect(GameObject player)
    {
        base.ApplyEffect(player);

        magText = GameObject.Find("MagText").GetComponent<TextMeshProUGUI>();

        //grab player controller script from playerrr
        PlayerController stats = player.GetComponent<PlayerController>();

        if (stats != null)
        {
            mags += 1;
            stats.fireRate *= 1.15f;
            magText.text = "x" + mags.ToString();
        }
    }
}
