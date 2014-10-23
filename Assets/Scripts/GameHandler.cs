using UnityEngine;
using System.Collections;

public class GameHandler : MonoBehaviour {

//states
	public enum State : byte {Menu,Started,Paused,Over};
	public static State gameState;

//player variables
	public static Player p1;
	public static Player p2;

	public static Transform p1Transform;
	public static Transform p2Transform;

	// Use this for initialization
	void Start () {
		gameState = State.Started;
		p1Transform = GameObject.Find("P1").transform;
		p2Transform = GameObject.Find("P2").transform;

		p1 = p1Transform.GetComponent<Player>();
		p2 = p2Transform.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		switch(gameState){
			case State.Menu:
				gameMenu();
			break;
			case State.Started:
				gameStarted();
			break;
			case State.Paused:
				gamePaused();
			break;
			case State.Over:
				gameOver();
			break;
			default:
				Debug.Log("State out of bounds!! pausing");
				gameState = State.Paused;
			break;
		}
	}

//private

	void gameMenu(){
		
	}

	void gameStarted(){
		if(p1.health <= 0 && p2.health <= 0)
			gameState = State.Over;
	}

	void gamePaused(){
		//check for unpausing:

		//freeze
		Time.timeScale = 0;
	}

	void gameOver(){

		Time.timeScale = 0;
	}

//public


}
