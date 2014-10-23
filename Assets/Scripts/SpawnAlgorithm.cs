using UnityEngine;
using System.Collections;

// This script calculates a position (in the edge of a circle) from a random value, and then initiates an enemy prefab at that position.


public class SpawnAlgorithm : MonoBehaviour {
	
	public Transform enemyPrefab;
	
	int horde;
	int checkMonsterCount;
	int maxCount;
	int rad;

	bool check = true;

	float startDelay;
	float timer;
	float diffMult;

	ArrayList monsters = new ArrayList();
	
	// Use this for initialization
	void Start () {
		timer = 10.0f;
		rad = 50;
		horde = 0;
		diffMult = 1.2f;
		checkMonsterCount = 5;
		maxCount = 1000;
		/*Debug.Log (p1);
		Transform g = Instantiate (enemyPrefab, Vector2.zero, Quaternion.identity) as Transform;
		g.GetComponent<Enemy_AI> ().setPlayers (p1, p2);*/
	}
	
	// Update is called once per frame
	void Update () {

		if (Time.time > startDelay && check) {
			Spawner (horde, checkMonsterCount);
			check = false;
		}

		if (Time.time > timer) {
			difficultyScalerWaveSender();
		}
		
	}



	void difficultyScalerWaveSender(){ //Makes the wave count 20% larger, as well as spawns another wave
		timer += 60.0f;
		checkMonsterCount =(int)(checkMonsterCount * diffMult);
		if(checkMonsterCount > maxCount) checkMonsterCount = maxCount;
		Spawner (horde, checkMonsterCount);
	}

	void Spawner(int num, int check){ //Simple spawner code, spawns currently check amount of units, spawns monsters on a random position in a circle.
		while (num < check) {
			
			int rn = Random.Range (0,10);
			Transform t = Instantiate (enemyPrefab, new Vector2(Mathf.Cos ((float)rn)*rad, Mathf.Sin ((float)rn)*rad), Quaternion.identity) as Transform;
			t.parent = transform;
			monsters.Add (t);
			num++;
		}
	}
}
