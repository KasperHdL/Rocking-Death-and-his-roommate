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

		public float dashForce = 40000f;
		private float dashUse = .51f;

//sticks
	public Transform leftStick;
	public Transform rightStick;

//attack
	public Transform attackCone;
	public bool attacking;
	public bool directingAttack;


	private float attackAngle;
	private float endAttack;
	private float attackLength = 2f;

	private Queue<AttackElement> attackQueue;

	//attack sequence particles
		public ParticleSystem ps;
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[16];

	//attack Sequences
		AttackSequence[] sequences = new AttackSequence[2]{	
				// seq params = float dmg, float time, float mult, params AttackElement[] attacks
				// ele params = bool isLeft, float start, float range
				new AttackSequence(1f,2f,1f,
					new AttackElement(true,-PI/2,PI),
					new AttackElement(true,-PI/2,PI)
				),
				new AttackSequence(2f,2f,2f,
					new AttackElement(false,-PI,PI*1.5f),
					new AttackElement(true,-PI/2,PI)
				),

			};

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
		attackQueue = new Queue<AttackElement>();
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
			if((right.magnitude != 0 || left.magnitude != 0) && energy > dashUse && Input.GetButtonDown(name + " L1")){
				energy -= dashUse;
				trail.time = 0.5f;
				trailEnd = Time.time + trailLength;
				rigidbody.AddForce(((right.magnitude == 0) ? left:right) * Time.deltaTime * dashForce);
			}
			movePlayer();
			checkAttackButtons();
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

//private
	/// <summary>attacks in direction</summary>
	/// <param name="dPad">direction of button pressed (0-7, 0 is up, clockwise)</param>
	private void startAttack(int dPad){
		//debug line
		anim.SetBool("moving",false);
			dPad = Mathf.Clamp(dPad,0,sequences.Length-1);
		attacking = true;
		endAttack = Time.time + attackLength;
		attackAngle = rightAngle;
		//Debug.Log("Attacks with dPad " + dPad + " attack" + " angle: " + attackAngle);

		//queue a sequence
		queueSequence(dPad);

		StartCoroutine(checkAttackElement(dPad));
	}

	private void queueSequence(int d){

		for(int i = 0;i<sequences[d].attElements.Length;i++)
			attackQueue.Enqueue(sequences[d].attElements[i]);

	}

	/// <summary>the player attacks in current direction
	private void attack(int dPad){
//		Debug.Log("Attacks with dPad " + direction + " attack" + " angle: " + attackAngle);
		Transform t = Instantiate(attackCone, transform.position + (attackCone.localScale.y/1.75f) * new Vector3(Mathf.Cos(attackAngle),0f,Mathf.Sin(attackAngle)),Quaternion.Euler(0,(-(attackAngle - PI/2)/PI)*180,0)) as Transform;
		t.position += Vector3.up * ((name == "P2") ? 1f:2f);
        Physics.IgnoreCollision(t.collider, collider);
	}

	/// <summary>Moves the player</summary>
	private void movePlayer(){
		//if attacking
		rigidbody.AddForce((left + right * 0.3f) * Time.deltaTime * acc);
		if(right.x > 0 && mirrored || (right.x < 0 && !mirrored)){
			model.localScale = new Vector3(-1 * model.localScale.x,1f,1f);
			mirrored = !mirrored;
		}
		if(left.magnitude != 0) 
			anim.SetBool("moving",true);
		else
			anim.SetBool("moving",false);
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

	///<summary>checks each frame if a current attack sequence is completed</summary>
	IEnumerator checkAttackElement(int direction) {
		bool sequenceDone = false;
		AttackElement att = attackQueue.Peek();
		bool isLeft = att.left;

		float startAngle = att.startAngle;
		float endAngle = att.endAngle;

		float angle = 0;

		float deltaAngle = endAngle - startAngle;
		float step = PI/8;
		float checkRange = step/2;
		int num = Mathf.RoundToInt(deltaAngle/step);

		float[] angles = new float[num];
		for(int i = 0;i<num;i++){
			float angleStep = step*i;
			angles[i] = startAngle + angleStep;
			particles[i].position = new Vector3(Mathf.Cos(startAngle + angleStep) * offset,0f,Mathf.Sin(startAngle + angleStep) * offset);
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
				attackQueue.Clear();

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
				//Debug.Log("seq left " + attackQueue.Count);
				if(attackQueue.Count == 0){
					attacking = false;
					attack(direction);
					anim.Play("Attack");

					zeroParticleSize();
				}else
					StartCoroutine(checkAttackElement(direction));

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

//structs

public struct AttackElement{
	public bool left;
	public float startAngle;
	public float endAngle;

	public AttackElement(bool isLeft){
		left = isLeft;
		float range = Random.Range(.5f,1f) * Mathf.PI;
		startAngle = Random.Range(-Mathf.PI,Mathf.PI-range);
		endAngle = startAngle + range;
	}

	public AttackElement(bool isLeft, float start, float range){
		left = isLeft;
		range = Mathf.Clamp(range,0,2*Mathf.PI);
		startAngle = Mathf.Clamp(start,-Mathf.PI,Mathf.PI - range);
		endAngle = startAngle + range;
	}
}

public struct AttackSequence{
	public AttackElement[] attElements;
	public float damage;
	public float maxCompleteTime;
	public float timeMultiplier;

	public AttackSequence(float dmg,float time,float mult, params AttackElement[] attacks){
		damage = dmg;
		maxCompleteTime = time;
		timeMultiplier = mult;

		attElements = attacks;
	}
}