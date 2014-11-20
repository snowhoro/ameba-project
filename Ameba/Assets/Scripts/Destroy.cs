using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {

    public float destroyTime = 2f;

	void Start () 
    {
        Destroy(gameObject, destroyTime);
	}
}
