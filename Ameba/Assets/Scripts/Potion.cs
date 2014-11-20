using UnityEngine;
using System.Collections;

public class Potion : Item {

    public int hitPoints = 50;

    public override void Use()
    {   
        if(number > 0)
        {
            Combat.instance.Heal(Player.instance, hitPoints);
            number--;
        }
    }
}
