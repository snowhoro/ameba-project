using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public float followSpeed = 1.0f;
    public float zDistance = 10.0f;
    public Transform playerPos;
	
    void Update()
    {
        if (playerPos == null)
            playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

	void LateUpdate () 
    {
        if (playerPos != null)
        {

            Vector3 tmp = new Vector3(playerPos.position.x, playerPos.position.y, zDistance * -1);
            Vector3 roundPos = Vector3.Lerp(transform.position, tmp, followSpeed * Time.deltaTime);

            roundPos = new Vector3(RoundToNearestPixel(roundPos.x, camera), RoundToNearestPixel(roundPos.y, camera), roundPos.z);
            transform.position = roundPos;
        }
	}

    public static float RoundToNearestPixel(float unityUnits, Camera viewingCamera)
    {
        float valueInPixels = (Screen.height / (viewingCamera.orthographicSize * 2)) * unityUnits;
        valueInPixels = Mathf.Round(valueInPixels);
        float adjustedUnityUnits = valueInPixels / (Screen.height / (viewingCamera.orthographicSize * 2));
        return adjustedUnityUnits;
    }
}