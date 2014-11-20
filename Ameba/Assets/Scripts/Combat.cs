using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Combat : MonoBehaviour
{

    public GameObject dmgNumbers;
    private static Combat _instance;

    public static Combat instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Combat>();

                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }

    private bool Hit(Base target, Base origin)
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

    public void DealDamage(Base target, Base origin)
    {
        if (Hit(target, origin))
        {
            int dmg = origin.AttackDamage - target.Defense;
            target.HitPoints -= dmg;
            ShowDamage(target,dmg);            
        }
        else
        {
            ShowDamage(target);
        }
    }

    private void ShowDamage(Base target, int damage)
    {
        TextMesh textmesh = ((GameObject)Instantiate(dmgNumbers, target.transform.position + new Vector3(0f, 0.5f, -3f), Quaternion.identity)).GetComponent<TextMesh>();
        textmesh.text = damage.ToString();

        if (target.tag != "Player")
            textmesh.color = Color.white;
        else
            textmesh.color = Color.red;       
    }

    private void ShowDamage(Base target)
    {
        TextMesh textmesh = ((GameObject)Instantiate(dmgNumbers, target.transform.position + new Vector3(0f, 0.5f, -3f), Quaternion.identity)).GetComponent<TextMesh>();
        textmesh.text = "MISS";
        textmesh.color = Color.cyan;
    }

    public void Heal(Base target, int hitPoints)
    {
        target.HitPoints += hitPoints;
        if (target.HitPoints > target.MaxHitPoints)
            target.HitPoints = target.MaxHitPoints;
    }
}
