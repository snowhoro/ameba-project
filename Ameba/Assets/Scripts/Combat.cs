using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public static class Combat
{


    public static bool Hit(Base target, Base origin)
    {
        int MissChance;
        int rnd;
        float hitChance = origin.Accuracy / target.Agility;

        if (hitChance < 1)
        {
            MissChance = (int)Mathf.Pow(hitChance, -1);
            rnd = Random.Range(0, MissChance);
            return rnd == 0 ? true : false;
        }
        else
        {
            rnd = Random.Range(0, (int)hitChance);
            return rnd == 0 ? false : true;
        }
    }

    public static void DealDamage(Base target, Base origin)
    {
        if (Hit(target, origin))
        {
            target.HitPoints -= origin.AttackDamage - target.Defense;
        }
        //else
        //print (origin.name + " ataco a : " + target.name + "y fallo");
    }
}
