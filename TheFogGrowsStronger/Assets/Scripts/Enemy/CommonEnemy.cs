using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonEnemy : Enemy
{
   public void TakeDamage()
    {
        ApplyDamage();
    }

    private void Update()
    {
        //check for if you can attack player
        
    }
}
