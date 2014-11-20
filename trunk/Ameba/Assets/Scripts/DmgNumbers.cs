using UnityEngine;
using System.Collections;

public class DmgNumbers : MonoBehaviour {

    public float topPosition = 1.0f;
    public float speed = 1.0f;
    public float fadeSpeed = 0.10f;
    private Vector3 endPos;

	void Start () 
    {
        endPos = transform.position + new Vector3(0, topPosition, 0);
	}
	
	void Update () 
    {
        transform.position = Vector3.Lerp(transform.position, endPos, speed * Time.deltaTime);
        Color temp = GetComponent<TextMesh>().color;
        temp.a -= fadeSpeed;
        GetComponent<TextMesh>().color = temp;
	}
}