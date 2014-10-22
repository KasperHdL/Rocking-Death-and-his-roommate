using UnityEngine;
using System.Collections;

// This script calculates a position (in the edge of a circle) from a random value, and then initiates an enemy prefab at that position.


public class SpawnAlgorithm : MonoBehaviour {
	
	public Transform enemyPrefab;
	
	Transform p1;
	Transform p2;
	
	int horde;
	int checkMonsterCount;
	int rad;

	bool check = true;

	float startDelay;
	float timer;
	float diffMult;

	ArrayList monsters = new ArrayList();
	
	private int rnd;
	
	// Use this for initialization
	void Start () {
		startDelay = 10.0f;
		timer = 60.0f;
		rad = 100;
		horde = 0;
		diffMult = 1.2f;
		checkMonsterCount = 20;
		p1 = GameObject.Find ("P1").transform;
		p2 = GameObject.Find ("P2").transform;
		/*Debug.Log (p1);
		Transform g = Instantiate (enemyPrefab, Vector2.zero, Quaternion.identity) as Transform;
		g.GetComponent<Enemy_AI> ().setPlayers (p1, p2);*/
	}
	
	// Update is called once per frame
	void Update () {

		if (Time.time > startDelay && check) {
			Spawner (horde, rnd, checkMonsterCount);
			check = false;
		}

		if (Time.time > timer) {
			Debug.Log ("HEEEEY" + checkMonsterCount);
			difficultyScalerWaveSender(ref diffMult, ref checkMonsterCount, ref timer);
		}
		
	}



	void difficultyScalerWaveSender(ref float _diffMult, ref int _checkMonsterCount, ref float _timer){ //Makes the wave count 20% larger, as well as spawns another wave
		_checkMonsterCount =(int)(_checkMonsterCount * diffMult);
		Spawner (horde, rnd, checkMonsterCount);
		_timer = _timer + 60.0f;
	}

	void Spawner(int num, int rn, int check){ //Simple spawner code, spawns currently check amount of units, spawns monsters on a random position in a circle.
		while (num < check) {
			rn = Random.Range (0,360);
			Transform t = Instantiate (enemyPrefab, new Vector2(Mathf.Cos ((float)rn)*rad, Mathf.Sin ((float)rn)*rad), Quaternion.identity) as Transform;
			t.GetComponent<Enemy_AI> ().setPlayers (p1, p2);
			transform.parent = t;
			monsters.Add (t);
			num++;
		}
	}
}
