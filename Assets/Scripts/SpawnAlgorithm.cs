using UnityEngine;
using System.Collections;

// This script calculates a position (in the edge of a circle) from a random value, and then initiates an enemy prefab at that position.


public class SpawnAlgorithm : MonoBehaviour {

	public Transform enemyPrefab;

	public Transform p1;
	Transform p2;

	int rnd;

	// Use this for initialization
	void Start () {
		p1 = GameObject.Find ("P1").transform;
		p2 = GameObject.Find ("P2").transform;
		Debug.Log (p1);
		rnd = Random.Range (0, 1);
		Transform g = Instantiate (enemyPrefab, new Vector2 (150, 2), Quaternion.identity) as Transform;
		g.GetComponent<Enemy_AI> ().setPlayers (p1, p2);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
