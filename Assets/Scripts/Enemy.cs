using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

//player reference
	private Transform p1;
	private Transform p2;

//attr
	public float monsterRange;
	private int acc = 1000;
	float health = 1;

//attack
	float attDelay = 0.2f;
	float nextAttack = 0f;
	float damage = 0.20f;

	void Start () {// ----------------------------------------------------------------------------------------------------------------------------START OF START
		p1 = GameHandler.p1Transform;
		p2 = GameHandler.p2Transform;
	}// ------------------------------------------------------------------------------------------------------------------------------------------END OF START
	
	// Update is called once per frame
	void Update () { // --------------------------------------------------------------------------------------------------------------------------START OF UPDATE
		Vector2 vec = returnClosestPlayerPos (p1.transform.position, p2.transform.position);
		transform.rotation = Quaternion.LookRotation(vec,-Vector3.forward);

		if (vec.magnitude > monsterRange) {
			vec.Normalize ();
			rigidbody2D.AddForce (vec * acc * Time.deltaTime);
		}else if(nextAttack < Time.time){
			nextAttack = Time.time + attDelay;
		    RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward, monsterRange,1<<10);
		    if(hit.collider != null)
		    	attack(hit.transform);
		}


	}// ------------------------------------------------------------------------------------------------------------------------------------------END OF UPDATE

//private

	private Vector2 returnClosestPlayerPos (Vector2 one, Vector2 two){//Returns the closest player's position
		Vector2 deltaOne = one-(Vector2)transform.position;
		Vector2 deltaTwo = two-(Vector2)transform.position;
		
		if (deltaOne.magnitude <= deltaTwo.magnitude) {
			return deltaOne;
		} else {
			return deltaTwo;
		}
	}

	
	private void attack(Transform player){//Takes a player and removes an amount from his health
		player.GetComponent<Player> ().takeDamage (damage);
	}


//public

	public void takeDamage (float dmg){// calculates damage on the enemy, and destroys it if health is below 0.
		health =- dmg;
		
		if (health <= 0) {
			Destroy(gameObject);		
		}
		
	}

}// ----------------------------------------------------------------------------------------------------------------------------------------------END OF MAIN