using UnityEngine;
using System.Collections;

public class EnemyGroup : MonoBehaviour {

	// Number of enemies in Group
	public float spawnTime_start;			// Time in seconds until spawning enemy pattern
	public GameObject[] enemies;
	public GameObject dropType;

	public float spawnRate = 0;				// Enemy spawn rate, leave 0 if none 

	int destroyed;		// Initial "Death Count"
	int lastDestroyed;	// Last destroyed enemy (starting at 0 index)
	int killCounter;	// Kill counter for enemies actually destroyed by player (instead of leaving the game)
	Vector3 lastDestroyed_coordinates;	// Coordinates of last enemy destroyed

	int allowSpawning = 0;	// 0 = No, 1 = Yes, 2 = Finished

	// Use this for initialization
	void Start () {
		// wait a certain amount of time until spawning enemies in pattern
		destroyed = 0;
		lastDestroyed = 0;

		// If spawn rate of enemies = 0, spawn all enemies with no time difference
		// Otherwise spawn them in intervals
		//StartCoroutine(WaitUntilStart(SpawnTime_start));
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad > spawnTime_start && allowSpawning == 0) {
			allowSpawning = 1;
		}

		if (allowSpawning == 1) {
			StartCoroutine (SpawnEnemy (spawnRate));
			allowSpawning = 2;	// Confirm finished
		} else {
			// Do nothing
		}
		// Check if entire group is destroyed.
		// If entire group destroyed, spawn a power-up on last-defeated enemy location

		OnDestruction ();
	}

	IEnumerator WaitUntilStart(float startTime) {
		yield return new WaitForSeconds (startTime);
	}

	IEnumerator SpawnEnemy(float var_spawnRate) {
	//void SpawnEnemy(float var_spawnRate) {
		// Set all animations to stopped by default
		if (Time.timeSinceLevelLoad > spawnTime_start) {
			for (int i = 0; i < enemies.Length; i++) {
				//Instantiate(enemies[i], enemies[i].transform.position, enemies[i].transform.rotation); // as GameObject;
				enemies [i].SetActive(true);

				enemies [i].GetComponent<Animator> ().SetTrigger ("SpawnEnemy");
				yield return new WaitForSeconds (var_spawnRate);
			}
		}
	}

	void OnDestruction() {
		for (int i = 0; i < enemies.Length; i++) {
			if (enemies [i].Equals (null)) {
				destroyed++;
			}
			if (enemies [i].Equals (null) != true) {
				lastDestroyed = i;	// Index of last remaining enemy in group
				lastDestroyed_coordinates = enemies [lastDestroyed].transform.position;
			}
		}

		if (destroyed != enemies.Length) {
			destroyed = 0;
		} else {
			// Drop power-up in last position where enemy was destroyed
			if(dropType == true) {
				Debug.Log(lastDestroyed_coordinates);
				
				GameObject clone = Instantiate (dropType, lastDestroyed_coordinates, Quaternion.Euler(0.0f,0.0f,0.0f)) as GameObject;
				clone.transform.parent = GameObject.Find("StageScroll_2D").transform;
			}

			//Instantiate (dropType, lastDestroyed_coordinates, Quaternion.Euler(0.0f,0.0f,0.0f));
			Destroy (gameObject);			// Remove game object from game
		}
	}
}