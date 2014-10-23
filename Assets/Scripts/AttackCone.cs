using UnityEngine;
using System.Collections;

public class AttackCone : MonoBehaviour {

	float startTime;
	float lifeSpan = 1f;
	BoxCollider2D coll;


	// Use this for initialization
	void Start () {
		startTime = Time.time;
		coll = collider2D as BoxCollider2D;
		Destroy(gameObject,lifeSpan);
	}
	
	// Update is called once per frame
	void Update () {
		float t = Time.time - startTime;
		if(t < .25f){
			coll.size = new Vector2(0.25f,0.25f);
			coll.center = new Vector2(0f,-0.375f);
		}else if(t < .5f){
			coll.size = new Vector2(0.5f,0.25f);
			coll.center = new Vector2(0f,-.125f);
		}else if(t < .75f){
			coll.size = new Vector2(0.75f,0.25f);
			coll.center = new Vector2(0f,.125f);
		}else{
			coll.size = new Vector2(1f,0.25f);
			coll.center = new Vector2(0f,0.375f);
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.layer == 9){
			col.gameObject.GetComponent<Enemy>().takeDamage(1f);
		}
		//Debug.Log(col.gameObject.name);
	}
}
