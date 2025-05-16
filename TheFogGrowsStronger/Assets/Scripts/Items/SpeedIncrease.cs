using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedIncrease : Item
{
    private TextMeshProUGUI syringeText;
    //Static so that it doesn't keep resetting itself when spawned in
    private static int syringes = 0;

    public override void ApplyEffect(GameObject player)
    {
        base.ApplyEffect(player);

        syringeText = GameObject.Find("SyringeText").GetComponent<TextMeshProUGUI>();

        //grab player controller script from playerrr
        PlayerController stats = player.GetComponent<PlayerController>();

        if (stats != null)
        {
            syringes += 1;
            //can have this affect walking and sprinting seperately for different items
            //in different functions or something
            //this just affects them both for now
            stats.MoveSpeed *= speedScaling;
            syringeText.text = "x" + syringes.ToString();
        }
    }
}
