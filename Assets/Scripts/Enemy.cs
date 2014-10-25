using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public SpawnAlgorithm spawner;
	public Transform model;

//player reference
	private Transform p1;
	private Transform p2;

	private Player player1;
	private Player player2;

//attr
	public float monsterRange;
	private int acc = 1000;
	float health = 1;

//attack
	float attDelay = 0.4f;
	float nextAttack = 0f;
	float damage = 0.10f;

	void Start () {// ----------------------------------------------------------------------------------------------------------------------------START OF START
		p1 = GameHandler.p1Transform;
		p2 = GameHandler.p2Transform;

		player1 = GameHandler.p1;
		player2 = GameHandler.p2;
	}// ------------------------------------------------------------------------------------------------------------------------------------------END OF START
	
	// Update is called once per frame
	void Update () { // --------------------------------------------------------------------------------------------------------------------------START OF UPDATE
		Vector3 vec = returnClosestPlayerPos (p1.transform.position, p2.transform.position);
		if(!player1.alive && player2.alive)vec = p2.transform.position - transform.position;
		else if(!player2.alive && player1.alive)vec = p1.transform.position - transform.position;
		
		model.localScale = new Vector3(Mathf.Sign(vec.x),1f,1f);

		if (vec.magnitude > monsterRange) {
			vec.Normalize ();
			rigidbody.AddForce (vec * acc * Time.deltaTime);
		}else if(nextAttack < Time.time){
			nextAttack = Time.time + attDelay;
		    RaycastHit hit;
		    Physics.Raycast(transform.position, vec, out hit, monsterRange,1<<10);
		    if(hit.collider != null)
		    	attack(hit.transform);
		}


	}// ------------------------------------------------------------------------------------------------------------------------------------------END OF UPDATE

//private

	private Vector3 returnClosestPlayerPos (Vector3 one, Vector3 two){//Returns the closest player's position
		Vector3 deltaOne = one-transform.position;
		Vector3 deltaTwo = two-transform.position;
		
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
			spawner.numEnemies--;
			Destroy(gameObject);		
		}
		
	}

}// ----------------------------------------------------------------------------------------------------------------------------------------------END OF MAIN