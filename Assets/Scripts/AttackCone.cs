using UnityEngine;
using System.Collections;

public class AttackCone : MonoBehaviour {

	private Camera cam;

	float startTime;
	int numFrames = 3;
	float lifeSpan = 1f;

	float step;

	public Material originalTex;
	private Material tex;

	public Transform vert;
	public Transform hori;


	// Use this for initialization
	void Start () {
		cam = Camera.main;
		step = lifeSpan/numFrames;
		tex = new Material(originalTex);

		hori.renderer.material = tex;
		vert.renderer.material = tex;

		startTime = Time.time;

		Destroy(gameObject,lifeSpan);
	}
	
	// Update is called once per frame
	void Update () {
		float t = Time.time - startTime;
		if(t < step){
			tex.mainTextureOffset = new Vector2(0f,0f);
		}else if(t < 2*step){
			tex.mainTextureOffset =  new Vector2(0.25f,0f);
		}else{
			tex.mainTextureOffset =  new Vector2(0.5f,0f);
		}

		Vector3 delta = transform.position - cam.transform.position;
		float a = Vector3.Angle(vert.forward, delta);

		if(Mathf.Abs(a) < 90f)
			vert.localScale = new Vector3(1f,1f,1f);
		else
			vert.localScale = new Vector3(1f,1f,-1f);
	}

	void OnTriggerEnter(Collider col){
		string tag = col.gameObject.tag;
		if(tag == "Enemy"){
			col.gameObject.GetComponent<Enemy>().takeDamage(1f);
		}else if(tag == "Player"){
			col.gameObject.GetComponent<Player>().heal(.25f);
		}
		//Debug.Log(col.gameObject.name);
	}
}
