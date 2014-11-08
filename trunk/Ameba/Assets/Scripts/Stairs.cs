using UnityEngine;
using System.Collections;

public class Stairs : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		//CAMBIAR ESTO!!!!!"!!!!!!!!!!!!!!!!!!
		GameObject.FindGameObjectWithTag ("Dungeon").GetComponent<DungeonGenerator> ().GenerateDungeon ();
	}
}
