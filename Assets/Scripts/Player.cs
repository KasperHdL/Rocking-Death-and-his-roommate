﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public ParticleSystem ps;
	ParticleSystem.Particle[] particles = new ParticleSystem.Particle[16];

	public Transform leftStick;
	public Transform rightStick;

//attack
	public bool attacking;

	private float endAttack;
	private float attackLength = 2f;

	private Queue<Attack> attackQueue;


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
		attackQueue = new Queue<Attack>();

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
		}
		debugStick();
		//Debug.Log("left: " + leftAngle + ", right: " + rightAngle);
	}

//private
	/// <summary>attacks in direction</summary>
	/// <param name="direction">direction of button pressed (0-7, 0 is up, clockwise)</param>
	private void startAttack(int direction){
		attacking = true;
		endAttack = Time.time + attackLength;


		attackQueue.Enqueue(new Attack(Random.value < .5f));
		attackQueue.Enqueue(new Attack(Random.value < .5f));
		attackQueue.Enqueue(new Attack(Random.value < .5f));
		attackQueue.Enqueue(new Attack(Random.value < .5f));
		StartCoroutine(checkAttackSeq(direction));
	}


	/// <summary>the player attacks in current direction
	private void attack(int direction){
		Debug.Log("Attacks with dPad " + direction + " attack");
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
			

			startAttack(dir);

		}else if(dY == -1){
			//down

			int dir = 4;
			if(dX == 1)
				dir--;
			else if(dX == -1)
				dir++;

			startAttack(dir);
		}else if(dX == 1){
			//right
			startAttack(2);
		}else if(dX == -1){
			//left
			startAttack(6);
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
		leftAngle = Mathf.Atan2(left.y,left.x);
		rightAngle = Mathf.Atan2(right.y,right.x);
	}

	/// <summary>moves the children object named left and rightStick</summary>
	private void debugStick(){
		//set position of debug sticks
		if(left.magnitude != 0) leftStick.position = transform.position + new Vector3(Mathf.Cos(leftAngle) * offset,Mathf.Sin(leftAngle) * offset,0);
		else leftStick.localPosition = Vector3.zero;

		if(right.magnitude != 0) rightStick.position = transform.position + new Vector3(Mathf.Cos(rightAngle) * offset,Mathf.Sin(rightAngle) * offset,0);
		else rightStick.localPosition = Vector3.zero;
	}

	///<summary>checks each frame if a current attack sequence is completed</summary>
	IEnumerator checkAttackSeq(int direction) {
		bool sequenceDone = false;
		Attack att = attackQueue.Peek();
		bool isLeft = att.left;

		float startAngle = att.startAngle;
		float endAngle = att.endAngle;

		float angle = 0;

		bool started = false;

		float deltaAngle = endAngle - startAngle;
		float step = Mathf.PI/8;
		float checkRange = step/4;
		int num = Mathf.RoundToInt(deltaAngle/step);

		float[] angles = new float[num];
		for(int i = 0;i<num;i++){
			float angleStep = step*i;
			angles[i] = startAngle + angleStep;
			particles[i].position = new Vector3(Mathf.Cos(startAngle + angleStep) * offset,Mathf.Sin(startAngle + angleStep) * offset,0f);
			particles[i].color = (isLeft ? Color.blue:Color.red);
			particles[i].size = .5f;
		}

		ps.SetParticles(particles,num);

		int pointsHit = 0;

		while(!sequenceDone) {
			if(isLeft)
				angle = leftAngle;
			else
				angle = rightAngle;

			if(Input.GetButton(name + " O")) {
				attacking = false;
				sequenceDone = true;

				zeroParticleSize();
			}
			for(int i = 0;i<num;i++){
				if(angles[i] != -1 && angle > angles[i] - checkRange && angle < angles[i] + checkRange){
					pointsHit++;
					angles[i] = -1;
					particles[i].color = Color.green;
					ps.SetParticles(particles,num);
				}
			}
			if(pointsHit == num){
				attackQueue.Dequeue();
				Debug.Log("seq left " + attackQueue.Count);
				if(attackQueue.Count == 0){
					attacking = false;
					attack(direction);

					zeroParticleSize();
				}else
					StartCoroutine(checkAttackSeq(direction));

				sequenceDone = true;
			}

			yield return new WaitForEndOfFrame();
		}
	}

	///<summary>sets the particles to zero</summary>
	private void zeroParticleSize(){	
		for(int i = 0;i<particles.Length;i++)
			particles[i].size = 0f;

		ps.SetParticles(particles,particles.Length);
	}

}

public struct Attack{
	public bool left;
	public float startAngle;
	public float endAngle;

	public Attack(bool left){
		this.left = left;
		float range = Random.Range(.5f,2f) * Mathf.PI;
		startAngle = Random.Range(-Mathf.PI,Mathf.PI-range);
		endAngle = startAngle + range;
	}
}
