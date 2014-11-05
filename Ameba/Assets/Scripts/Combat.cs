using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Combat : MonoBehaviour {

	private List<Combat> nearbyEnemies;
	private List<Combat> turnList;
	
	private static Combat _instance;
	
	public static Combat instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<Combat>();
				
				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}
	
	void Awake() 
	{
		if(_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			if(this != _instance)
				Destroy(this.gameObject);
		}
		
		nearbyEnemies = new List<Combat>();
		turnList = new List<Combat>();
	}

	public bool Hit(Base target, Base origin)
	{
		int MissChance;
		int rnd;
		float hitChance = origin.Accuracy / target.Agility;

		if (hitChance < 1) 
		{				
			MissChance = (int)Mathf.Pow (hitChance, -1);
			rnd = Random.Range (0, MissChance);
			return rnd == 0 ? true : false;
		} 
		else 
		{
			rnd = Random.Range (0, (int)hitChance);
			return rnd == 0 ? false : true;
		}
	}

	public void DealDamage(Base target, Base origin)
	{
		if (Hit (target, origin)) {
						print (origin.name + " ataco a : " + target.name + " HIT");
						target.HitPoints -= origin.AttackDamage - target.Defense;
				}
		else
			print (origin.name + " ataco a : " + target.name + "y fallo");
	}
}
