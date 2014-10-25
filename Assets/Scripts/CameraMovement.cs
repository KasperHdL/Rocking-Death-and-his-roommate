using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Transform p1;
	public Transform p2;

	public Vector3 pos;

	private float padding = 300;

	private float minSize = 8;

	private float targetSize = 5;
	private float targetPos;

	private float increment = 0.1f;

	private Vector3 offset = new Vector3(0,15,-25);

	private Vector3 shake = new Vector3(0,0,0);
	private float shakeEnd = 0f;
	private float shakeLength = 1f;


	// Use this for initialization
	void Start () {
		//start Coroutine that lerps pos and size
	}
	
	// Update is called once per frame
	void Update () {
		/*
		//screen
		Vector3 w1 = camera.WorldToScreenPoint(p1.position);
		Vector3 w2 = camera.WorldToScreenPoint(p2.position);

		Vector3 delta = w2 - w1;
		float largestDelta = Mathf.Abs((Mathf.Abs(delta.x) < Mathf.Abs(delta.z)) ? delta.z:delta.x);
		float pixelSize = ((delta.x < delta.z) ? camera.pixelHeight:camera.pixelWidth);
		float pad = padding * pixelSize/500f;
		//Debug.Log("p1: " + w1 + ", p2: " + p2 + " delta: " + delta + " size: " + camera.orthographicSize + " cW " + camera.pixelWidth + ", cH " + camera.pixelHeight);
		if(targetSize >= minSize){
			if(largestDelta > pixelSize - pad)
				targetSize += increment;
			
			else if(largestDelta < pixelSize - pad)
				targetSize -= increment;
		}else
			targetSize = minSize;

		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, 0.1f);
*/

		
		if(shakeEnd < Time.time)
			shake = Vector3.zero;
		else
			shake = new Vector3(Random.Range(-.1f,.1f),Random.Range(-.1f,.1f),Random.Range(-.1f,.1f));

		//world
		Vector3 delta = p2.position - p1.position;
		pos = Vector3.Lerp(transform.position, p1.position + delta/2 - Vector3.forward*delta.z/20 +offset, 0.1f);
		transform.position = pos + shake;
	}

//public
	public void ScreenShake(){
		shakeEnd = Time.time + shakeLength;
	}
}
