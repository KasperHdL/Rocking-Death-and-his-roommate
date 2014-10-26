using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	private Camera cam;

	public Animator anim;

	private const float PI = Mathf.PI;

	public Transform model;
	public bool mirrored = false;

//health
	public float health = 1f;
	public Image healthCircle;

	public bool alive = true;

//energy
	private float energy = 1f;
	private float energyTick = 0.01f;
	public Image energyCircle;
	public RectTransform energyRect;

	//dash
		public TrailRenderer trail;
		private float trailEnd = 0f;
		private float trailLength = 0.5f;

		public float dashForce = 8000f;
		private float dashUse = .35f;

//sticks
	public Transform leftStick;
	public Transform rightStick;

//attack
	public Transform attackCone;
	public bool attacking;

	private Vector3 attackDirection;
	private float attackAngle;

	//attack sequence particles
		public Transform attackUI;
		public ParticleSystem ps;
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[16];

//movement
	private float acc = 4000f;
	

//axis control
	private float offset = 2;
	private float deadzone = 0f;

	private Vector3 left;
	private Vector3 right;

	private float leftAngle;
	private float rightAngle;

	// Use this for initialization
	void Start () {
		trail.time = 0;
		cam = Camera.main;

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
		if(!alive) return;
		getAxis();
		if(energy < 1f) energy += energyTick;
		else energy = 1f;

		if(trailEnd > Time.time){
			trail.time = trailEnd - Time.time;
		}else{
			trail.time = 0f;
		}

		if(!attacking && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")){
			if(left.magnitude != 0 && energy > dashUse && Input.GetButtonDown(name + " R1")){
				energy -= dashUse;
				trail.time = 0.5f;
				trailEnd = Time.time + trailLength;
				rigidbody.AddForce(left * Time.deltaTime * dashForce);
			}
			movePlayer();
			if(Input.GetButtonDown(name + " L1"))
				startAttack();
		}else if(attacking && Input.GetButtonUp(name + " L1")){
			attacking = false;
		}

		debugStick();
		if(energy != 1){
			energyCircle.enabled = true;
			energyCircle.fillAmount = energy;
			Vector3 wPos = cam.WorldToScreenPoint(transform.position);
			RectTransform r = energyRect;
			r.position = new Vector2(wPos.x + 31f,wPos.y + 41f);
			energyRect = r;
		}else{
			energyCircle.enabled = false;
		}
		//Debug.Log("left: " + leftAngle + ", right: " + rightAngle);
	}

//private

	/// <summary>Moves the player</summary>
	private void movePlayer(){
		//if attacking
		rigidbody.AddForce((left) * Time.deltaTime * acc);
		if(left.x > 0 && mirrored || (left.x < 0 && !mirrored)){
			model.localScale = new Vector3(-1 * model.localScale.x,1f,1f);
			mirrored = !mirrored;
		}
		if(left.magnitude != 0) 
			anim.SetBool("moving",true);
		else
			anim.SetBool("moving",false);
	}

	/// <summary>Gets the axis for the sticks and sets left and right stick and their angle</summary>
	private void getAxis(){
		left = new Vector3(Input.GetAxis(name + " Left Stick X"),0f,Input.GetAxis(name + " Left Stick Y"));
		right = new Vector3(Input.GetAxis(name + " Right Stick X"),0f,Input.GetAxis(name + " Right Stick Y"));

		//deadzone if attacking
		if(attacking)
			left = new Vector3(Mathf.Abs(left.x) < deadzone ? 0:left.x,0f,(Mathf.Abs(left.z) < deadzone ? 0:left.z));

		//angle
		leftAngle = Mathf.Atan2(left.z,left.x);
		rightAngle = Mathf.Atan2(right.z,right.x);
	}

	/// <summary>moves the children object named left and rightStick</summary>
	private void debugStick(){
		//set position of debug sticks
		if(left.magnitude != 0){
			leftStick.position = transform.position + new Vector3(Mathf.Cos(leftAngle) * offset,0f,Mathf.Sin(leftAngle) * offset);
			leftStick.rotation = Quaternion.LookRotation(left);

		}else leftStick.localPosition = Vector3.zero;

		if(right.magnitude != 0){
			rightStick.position = transform.position + new Vector3(Mathf.Cos(rightAngle) * offset,0f,Mathf.Sin(rightAngle) * offset);
			rightStick.rotation = Quaternion.LookRotation(right);

		}else rightStick.localPosition = Vector3.zero;

		//rotate
	}

	private void updateRightStick(){
		if(right.magnitude != 0){
			rightStick.position = transform.position + new Vector3(Mathf.Cos(rightAngle - attackAngle) * offset,0f,Mathf.Sin(rightAngle - attackAngle) * offset);
			rightStick.rotation = Quaternion.LookRotation(right);
		}else rightStick.localPosition = Vector3.zero;

	}

	///<summary>sets the particles to zero</summary>
	private void zeroParticleSize(){	
		for(int i = 0;i<particles.Length;i++)
			particles[i].size = 0f;

		ps.SetParticles(particles,particles.Length);
	}

	//ATTACK

	private void startAttack(){
		attacking = true;
		anim.SetBool("moving",false);
		StartCoroutine(waitForAttackDirection());
	}

	private void attack(){
		//actual attack
		attacking = false;
		anim.Play("Attack");
		zeroParticleSize();

		Transform t = Instantiate(attackCone, transform.position + (attackCone.localScale.y/((name == "P1") ? 1.5f:1.75f)) * new Vector3(Mathf.Cos(attackAngle),0f,Mathf.Sin(attackAngle)),Quaternion.Euler(0,(-(attackAngle - PI/2)/PI)*180,0)) as Transform;
		t.position += Vector3.up * ((name == "P2") ? 1f:2f);
        Physics.IgnoreCollision(t.collider, collider);
	}

	IEnumerator waitForAttackDirection(){
		while(right.magnitude == 0){
			if(Input.GetButton(name + " O")){
				Debug.Log("up");
				attacking = false;
				yield break;
			}
			yield return new WaitForEndOfFrame();
		}
		if(attacking == false)yield break;

		attackDirection = right;
		attackAngle = rightAngle;

		if(right.x > 0 && mirrored || (right.x < 0 && !mirrored)){
			model.localScale = new Vector3(-1 * model.localScale.x,1f,1f);
			mirrored = !mirrored;
		}

		StartCoroutine(updateAttack(120));
	}

	IEnumerator updateAttack(int angleAttackCone) { 

		float deltaAngle = ((float)angleAttackCone/180)*PI;
		float startAngle = -deltaAngle/2+PI/16;

		float step = PI/8;
		float checkRange = step/2;
		int num = Mathf.RoundToInt(deltaAngle/step);

		attackUI.rotation = Quaternion.Euler(0,(-attackAngle/PI)*180,0);

		float[] angles = new float[num];

		for(int i = 0;i<num;i++){
			float angleStep = step*i;
			float newAngle = startAngle + angleStep;
			
			if(newAngle < -PI)newAngle += 2*PI;
			if(newAngle > PI)newAngle -= 2*PI;
			angles[i] = newAngle;
			Debug.Log(newAngle);
			particles[i].position = new Vector3(Mathf.Cos(startAngle + angleStep) * offset,0f,Mathf.Sin(startAngle + angleStep) * offset);
			particles[i].color = Color.red;
			particles[i].size = .5f;
		}

		ps.SetParticles(particles,num);

		int pointsHit = 0;

		while(attacking){
			updateRightStick();

			float angle = rightAngle - attackAngle;

			if(angle < -PI)angle += 2*PI;
			if(angle > PI)angle -= 2*PI;

			if(Input.GetButton(name + " O")) {
				attacking = false;
				zeroParticleSize();
				yield break;
			}
			Debug.Log(angle);
			for(int i = 0;i<num;i++){
				if(angles[i] != -9 && angle > angles[i] - checkRange && angle < angles[i] + checkRange){
					pointsHit++;
					angles[i] = -9;
					particles[i].color = Color.green;
					ps.SetParticles(particles,num);
				}
			}

			if(pointsHit == num){
				attack();
				yield break;
			}

			yield return new WaitForEndOfFrame();
		}

		zeroParticleSize();
	}

//public
	/// <summary>player takes damage</summary>
	/// <param name="amount">the amount of damage taken - 1 is full health</param>
	public void takeDamage(float amount){
		health -= amount;
		if(health < 0f){
			//Dead
			alive = false;
			collider.enabled = false;
			transform.Rotate(90,0,0);
			rigidbody.useGravity = false;
			Debug.Log("dead");
			anim.SetBool("dead",true);
		}

		//update health Circle
		healthCircle.fillAmount = health;
	}

	public void heal(float amount){
		health += amount;
		if(health > 1f) health = 1f;
		healthCircle.fillAmount = health;
	}
}