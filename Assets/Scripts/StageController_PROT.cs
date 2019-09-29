using UnityEngine;
using System.Collections;

public class StageController_PROT : MonoBehaviour {

	public GameObject scrollStage;
	public GameObject[] enemyWave;
	public float[] activate_time;

	int wave_increment = 0;

	// Use this for initialization
	void Start () {
		// Ensure all waves are inactive at first
	}
	
	// Update is called once per frame
	void Update () {
		// Check if stage start time matches the next spawn
		if (wave_increment < enemyWave.Length) {
			if (Time.timeSinceLevelLoad >= activate_time [wave_increment]) {
				enemyWave [wave_increment].SetActive (true);

				enemyWave [wave_increment].transform.parent = scrollStage.transform;
				wave_increment++;
			}
		}
	}
}