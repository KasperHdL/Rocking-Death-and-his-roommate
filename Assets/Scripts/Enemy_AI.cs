using UnityEngine;
using System.Collections;

public class Enemy_AI : MonoBehaviour {

//player reference
	private bool playerSet = false;
	Transform playerOne;
	Transform playerTwo;

//attr
	public int monsterRange;
	private int acc = 1000;
	int health = 100;

	float attackP = 0.20f;

///<K> ved ikke om vi skal skrive beskeder til hinanden sådan her :P det er hvertfald en mulighed [du kan bare slette dem når du kan se det jeg har gjort (småting)]


	void Start () {// ----------------------------------------------------------------------------------------------------------------------------START OF START

	}// ------------------------------------------------------------------------------------------------------------------------------------------END OF START
	
	// Update is called once per frame
	void Update () { // --------------------------------------------------------------------------------------------------------------------------START OF UPDATE

		if (!playerSet)return; ///<K>if the playerSet is false then it return and therefore will not run the rest of update</K>

		Vector3 viewDir = (transform.TransformDirection (Vector3.forward));
		viewDir.z = 0;

		Debug.Log ("x: " + viewDir.x + "y: " + viewDir.y + "z: " + viewDir.z);

		Vector2 vec = returnClosestPlayerPos (playerOne.transform.position, playerTwo.transform.position);
		if (vec.magnitude > monsterRange) {///<K>uncommented this (set monsterRange = 2, will stop right in front of player)</K>
			vec.Normalize ();
			rigidbody2D.AddForce (vec * acc * Time.deltaTime);
		}else if (returnClosestPlayerPos (playerOne.transform.position, playerTwo.transform.position) == (Vector2)playerOne.position && 
		          Physics.Raycast(transform.position, viewDir, monsterRange)) {
			Debug.DrawLine(transform.position, viewDir, Color.blue, Mathf.Infinity);
			//attack (attackP, returnClosestPlayer(playerOne, playerTwo));
			Debug.Log ("P1 : IIIIII");
			attack (attackP, playerOne);
		} else if (returnClosestPlayerPos (playerOne.transform.position, playerTwo.transform.position) == (Vector2)playerTwo.position && 
		           Physics.Raycast(transform.position, viewDir, monsterRange)){
			Debug.Log ("P2 : AAAUUU");
			Debug.DrawLine(transform.position, viewDir, Color.blue, Mathf.Infinity);
			attack (attackP, playerTwo);
		}

		transform.LookAt (returnClosestPlayerPos (playerOne.transform.position, playerTwo.transform.position));
		/*
///Fandt ud af at der ikke er nogen grund til at bruge det her med den friction der er så kan vi bare slette det her :)
			if(rigidbody2D.velocity.magnitude > maxVel)
				rigidbody2D.velocity = vec * maxVel;
*/

	}// ------------------------------------------------------------------------------------------------------------------------------------------END OF UPDATE

	private Vector2 returnClosestPlayerPos (Vector2 one, Vector2 two){//Returns the closest player's position
		Vector2 deltaOne = one-(Vector2)transform.position;
		Vector2 deltaTwo = two-(Vector2)transform.position;
		
		if (deltaOne.magnitude <= deltaTwo.magnitude) {
			return deltaOne;
		} else {
			return deltaTwo;
		}
	}

	/*private Transform returnClosestPlayer (Transform one, Transform two){//Returns the closest player.
		Vector2 deltaOne = one.position-(Vector2)transform.position;
		Vector2 deltaTwo = two.position-(Vector2)transform.position;
		
		if (deltaOne.magnitude <= deltaTwo.magnitude) {
			return one;
		} else {
			return two;
		}
	}*/
	
	void unlife (ref int _health, int _damage){// calculates damage on the enemy, and destroys it if health is below 0.
		_health =- _damage;
		
		if (_health <= 0) {
			Debug.Log ("hey, im dead");
			Destroy(gameObject);		
		}
		
	}
	
	private void attack(float _attackP, Transform player){//Takes a player and removes an amount from his health


		player.GetComponent<Player> ().takeDamage (attackP);
	}

	public void setPlayers(Transform p1,Transform p2){
		playerSet = true;
		playerOne = p1;
		playerTwo = p2;
	}

	/*void OnCollisionEnter(Collision other){
		if (other.gameObject.name == "monster") {
			GameObject.Find("GM").GetComponent<MasterScript>().lives--;
			transform.position = new Vector3(0,0,0);
		}
	}*/
}// ----------------------------------------------------------------------------------------------------------------------------------------------END OF MAIN