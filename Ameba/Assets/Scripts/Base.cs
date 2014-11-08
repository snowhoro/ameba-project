using UnityEngine;
using System.Collections;

public class Base : MonoBehaviour 
{
    public int Level;
    public int Experience;

    public Vector2 Position;

    public int MaxHitPoints;
    public int HitPoints;

    public int MaxMagicPoints;
    public int MagicPoints;

    public int Strength;
    public int Constitution;
    public int Intelligence;
    public int Dexterity;
	public int Luck;
    
    public int AttackDamage;
	public int Defense;
	public int Accuracy;
	public int Agility;
	public int Speed;

    public int MaxStamina;
	public int Stamina;

    public int AttackCost;
    public int MoveCost;

    public virtual void Move(){}
    public virtual void Attack(){}

}
