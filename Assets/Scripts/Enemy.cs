using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public SpawnAlgorithm spawner;
	public Transform model;

	public Animator anim;
	private bool alive = true;
	private Vector3 beforeStunPos;
	private bool stunned = false;
	private float endStun = 0;
	private float stunLength = 1.7f;

//player reference
	private Transform p1;
	private Transform p2;

	private Player player1;
	private Player player2;

//attr
	public float monsterRange;
	private int acc = 900;
	float health = 1;

//attack
	float attDelay = 1f;
	float nextAttack = 0f;
	float damage = 0.21f;

	void Start () {// ----------------------------------------------------------------------------------------------------------------------------START OF START
		p1 = GameHandler.p1Transform;
		p2 = GameHandler.p2Transform;

		acc += Random.Range(0,200);

		player1 = GameHandler.p1;
		player2 = GameHandler.p2;
	}// ------------------------------------------------------------------------------------------------------------------------------------------END OF START
	
	// Update is called once per frame
	void Update () { // --------------------------------------------------------------------------------------------------------------------------START OF UPDATE
		if(stunned){
			if(endStun < Time.time){
				transform.position = beforeStunPos;
				stunned = false;
			}else{
				transform.position = beforeStunPos + new Vector3(Random.Range(-.1f,.1f),Random.Range(-.1f,.1f),Random.Range(-.1f,.1f));
				return;
			}
		}
		if(!alive) return;
		Vector3 vec = returnClosestPlayerPos (p1.transform.position, p2.transform.position);
		if(!player1.alive && player2.alive)vec = p2.transform.position - transform.position;
		else if(!player2.alive && player1.alive)vec = p1.transform.position - transform.position;

		model.localScale = new Vector3(Mathf.Sign(vec.x),1f,1f);

		if (vec.magnitude > monsterRange) {
			anim.SetBool("attacking",false);

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
		anim.SetBool("attacking",true);
		player.GetComponent<Player> ().takeDamage (damage);
	}


//public

	public void takeDamage (float dmg){// calculates damage on the enemy, and destroys it if health is below 0.
		if(stunned) return;
		health -= dmg;
		rigidbody.velocity = Vector3.zero;
		if (health <= 0) {
			spawner.numEnemies--;
			transform.Rotate(90,0,0);
			//Dead
			//anim.SetBool("attacking",false);
			anim.speed = 0;
			alive = false;
			collider.enabled = false;
			rigidbody.useGravity = false;
		}

		beforeStunPos = transform.position;
		stunned = true;

		endStun = Time.time + stunLength;
		
		
	}

}// ----------------------------------------------------------------------------------------------------------------------------------------------END OF MAIN