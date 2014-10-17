using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

	public Transform leftStick;
	public Transform rightStick;

	public bool attacking = false;

//movement
	private float acc = 4000f;
	private float maxVel = 40f;


//axis control
	private float offset = 2;
	private float deadzone = 0.5f;

	private Vector2 left;
	private Vector2 right;

	private float leftAngle;
	private float rightAngle;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		getAxis();
		movePlayer();

		debugSticks();
	}

//private

	private void movePlayer(){
		//if attacking
		rigidbody2D.AddForce(left * Time.deltaTime * acc);
		if(rigidbody2D.velocity.magnitude > maxVel)
			rigidbody2D.velocity = left * maxVel;

		if(right.magnitude != 0)
			transform.rotation = Quaternion.LookRotation(right);

	}

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


	private void debugSticks(){
		//set position of debug sticks
		if(left.magnitude != 0) leftStick.position = transform.position + new Vector3(Mathf.Cos(leftAngle) * offset,Mathf.Sin(leftAngle) * offset,0);
		else leftStick.localPosition = Vector3.zero;

		if(right.magnitude != 0) rightStick.localPosition = new Vector3(0,0,offset);
		else rightStick.localPosition = Vector3.zero;
	}
}
