using UnityEngine;
using System.Collections;

// This script calculates a position (in the edge of a circle) from a random value, and then initiates an enemy prefab at that position.


public class SpawnAlgorithm : MonoBehaviour {
	
	public Transform enemyPrefab;
	
	public int numEnemies;
	public float checkNum;
	int maxCount;

	bool check = true;

	float startDelay;
	float timer;
	float diffMult;
	
	// Use this for initialization
	void Start () {
		timer = 1.0f;
		numEnemies = 0;
		diffMult = 1.1f;
		checkNum = 2;
		maxCount = 200;
		/*Debug.Log (p1);
		Transform g = Instantiate (enemyPrefab, Vector2.zero, Quaternion.identity) as Transform;
		g.GetComponent<Enemy_AI> ().setPlayers (p1, p2);*/
	}
	
	// Update is called once per frame
	void Update () {

		if (Time.time > startDelay && check) {
			Spawner ();
			check = false;
		}

		if (Time.time > timer) {
			difficultyScalerWaveSender();
		}
		
	}



	void difficultyScalerWaveSender(){ //Makes the wave count 20% larger, as well as spawns another wave
		timer += 5.0f;
		Spawner ();
	}

	void Spawner(){ //Simple spawner code, spawns currently check amount of units, spawns monsters on a random position in a circle.
		if(numEnemies > maxCount)return;
		while (numEnemies < checkNum) {
			
			int rn = Random.Range (-2,3);

			Transform t = Instantiate (enemyPrefab, new Vector3(Mathf.Cos ((float)rn*Mathf.PI/2) * 70f,0f, Mathf.Sin ((float)rn*Mathf.PI/2) * 60f), Quaternion.identity) as Transform;
			t.parent = transform;
			t.GetComponent<Enemy>().spawner = this;
			numEnemies++;
		}
		checkNum = checkNum * diffMult;
	}
}
