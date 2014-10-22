using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Transform leftStick;
	public Transform rightStick;

//attack
	public bool attacking;

	private float endAttack;
	private float attackLength = 2f;


//movement
	private float acc = 4000f;


//axis control
	private float offset = 2;
	private float deadzone = 0f;

	private Vector2 left;
	private Vector2 right;

	private float leftAngle;
	private float rightAngle;

	// Use this for initialization
	void Start () {
		if(Settings.debug){
			leftStick.renderer.enabled = true;
			rightStick.renderer.enabled = true;
		}else{
			leftStick.renderer.enabled = false;
			rightStick.renderer.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		getAxis();

		if(!attacking){
			movePlayer();
			checkAttackButtons();
		}else{
			if(Time.time > endAttack){
				attacking = false;
				leftStick.localPosition = Vector3.zero;
				rightStick.localPosition = Vector3.zero;
			}else{
				//attack
			}
		}	
			debugStick();
	}

//private
	/// <summary>attacks in direction</summary>
	/// <param name="direction">direction of attack(0-7, 0 is up, clockwise)</param>
	private void attack(int direction){
		Debug.Log(direction);
		attacking = true;
		endAttack = Time.time + attackLength;
	}

	/// <summary>Moves the player</summary>
	private void movePlayer(){
		//if attacking
		rigidbody2D.AddForce(left * Time.deltaTime * acc);

		if(right.magnitude != 0)
			transform.rotation = Quaternion.LookRotation(right);

	}
	/// <summary>Checks if any attack button is pressed(or multiple) and activates attack in that direction</summary>
	private void checkAttackButtons(){
		float dX = Input.GetAxis(name + " dPad X");
		float dY = Input.GetAxis(name + " dPad Y");

		//Debug.Log("x " + dX + ", " + "y " + dY);

		if(dY == 1){
			//up
			int dir = 0;
			if(dX == 1)
				dir++;
			else if(dX == -1)
				dir += 7;
			

			attack(dir);

		}else if(dY == -1){
			//down

			int dir = 4;
			if(dX == 1)
				dir--;
			else if(dX == -1)
				dir++;

			attack(dir);
		}else if(dX == 1){
			//right
			attack(2);
		}else if(dX == -1){
			//left
			attack(6);
		}
	}

	/// <summary>Gets the axis for the sticks and sets left and right stick and their angle</summary>
	private void getAxis(){
		left = new Vector2(Input.GetAxis(name + " Left Stick X"),Input.GetAxis(name + " Left Stick Y"));
		right = new Vector2(Input.GetAxis(name + " Right Stick X"),Input.GetAxis(name + " Right Stick Y"));

		//deadzone if attacking
		if(attacking)
			left = new Vector2(Mathf.Abs(left.x) < deadzone ? 0:left.x,(Mathf.Abs(left.y) < deadzone ? 0:left.y));

		//angle
		leftAngle = -Mathf.Atan2(left.x,left.y) + Mathf.PI /2;
		rightAngle = -Mathf.Atan2(right.x,right.y) + Mathf.PI /2;
	}

	/// <summary>moves the children object named left and rightStick</summary>
	private void debugStick(){
		//set position of debug sticks
		if(left.magnitude != 0) leftStick.position = transform.position + new Vector3(Mathf.Cos(leftAngle) * offset,Mathf.Sin(leftAngle) * offset,0);
		else leftStick.localPosition = Vector3.zero;

		if(right.magnitude != 0) rightStick.position = transform.position + new Vector3(Mathf.Cos(rightAngle) * offset,Mathf.Sin(rightAngle) * offset,0);
		else rightStick.localPosition = Vector3.zero;
	}
}
