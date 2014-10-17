using UnityEngine;
using System.Collections;

public class Enemy_AI : MonoBehaviour {

//player reference
	private bool playerSet = false;
	Transform playerOne;
	Transform playerTwo;

//attr
	public int monsterRange;
	private int acc = 2000;
	private int maxVel = 20;
	int temp = 1;

	void Start () {// ----------------------------------------------------------------------------------------------------------------------------START OF START
		
	}// ------------------------------------------------------------------------------------------------------------------------------------------END OF START
	
	// Update is called once per frame
	void Update () { // --------------------------------------------------------------------------------------------------------------------------START OF UPDATE
		if (playerSet) {

			Vector2 vec = returnClosestPlayer (playerOne.transform.position, playerTwo.transform.position);
			//if (vec.magnitude > monsterRange) {
			vec.Normalize ();
			rigidbody2D.AddForce (vec * acc * Time.deltaTime);

			if(rigidbody2D.velocity.magnitude > maxVel){
				rigidbody2D.velocity = vec * maxVel;
			}
						/*	
			}
		} else if (vec == (Vector2)playerOne.position) {
			attack (ref temp);			

		} else if (vec == (Vector2)playerTwo.position) {
			attack (ref temp);			
		}*/
		}

	}// ------------------------------------------------------------------------------------------------------------------------------------------END OF UPDATE
	/// <summary>
	/// Returns the closest player.
	/// </summary>
	/// <returns>The closest player.</returns>
	/// <param name="one">One.</param>
	/// <param name="two">Two.</param>
	private Vector2 returnClosestPlayer (Vector2 one, Vector2 two){
		Vector2 deltaOne = one-(Vector2)transform.position;
		Vector2 deltaTwo = two-(Vector2)transform.position;
		
		if (deltaOne.magnitude <= deltaTwo.magnitude) {
			return deltaOne;
		} else {
			return deltaTwo;
		}
	}

	/// <summary>
	/// Attack the specified x.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	private void attack(ref int x){//Takes a player and removes a point from his health
		x--;
	}

	public void setPlayers(Transform p1,Transform p2){
		playerSet = true;
		playerOne = p1;
		playerTwo = p2;
	}
}// ----------------------------------------------------------------------------------------------------------------------------------------------END OF MAIN