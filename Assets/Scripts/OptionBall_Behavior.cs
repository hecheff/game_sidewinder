// Behavior program for Option Balls

using UnityEngine;
using System.Collections;

public class OptionBall_Behavior : MonoBehaviour {
	public Transform shotSpawner;			// Main shot spawner
	public Transform shotSpawner_double;	// Double Gun

	GameObject player;
	private PlayerController playerController;

	GameObject shot;
	GameObject shot_double;
	GameObject shot_laser;

	float fireRate;
	float nextFire;

	string attackType;
	int missile_status;
	int weaponMagnitude;

	// Use this for initialization
	void Start () {
		// Get all behaviors from player controller
		player = GameObject.Find("Player");
		if (player != null) {
			playerController = player.GetComponent<PlayerController>();
		} if (player == null) { Debug.Log ("Cannot find 'PlayerController' script."); }
	}
	
	// Update is called once per frame
	void Update () {
		checkPlayerStats ();
	}

	void FixedUpdate() {
		FiringSystem ();	// Constantly check firing function
	}

	// Weapon firing mechanics
	public void FiringSystem () {
		// Fire weapon based on current type
		bool autoFire = playerController.autoFire;

		switch (attackType) {
		case "DOUBLE":
				// Double shot mode
				if (autoFire == false) {
					if (Input.GetButton ("Fire_Main") && Time.time > nextFire) {
						Fire_Double_Default ();
					}
				} else {
					if (Time.time > nextFire) {
						Fire_Double_Default ();
					}
				}
				break;
			case "LASER":
				// For Ripple Laser
				if (autoFire == false) {
					if (Input.GetButton ("Fire_Main") && Time.time > nextFire) {
					Fire_Laser_Ripple ();
					}
				} else {
					if (Time.time > nextFire) {
					Fire_Laser_Ripple ();
					}
				}
				break;

			case "SHOT":
			default:
				// Basic shot
				if (autoFire == false) {
					if (Input.GetButton ("Fire_Main") && Time.time > nextFire) {
					Fire_Shot_Default ();
					}
				} else {
					if (Time.time > nextFire) {
					Fire_Shot_Default ();
					}
				}
				break;
		}
	}
	public void Fire_Shot_Default() {
		nextFire = Time.time + fireRate;
		Instantiate(shot, shotSpawner.position, transform.rotation); 								// Primary shot (basic)
	}
	public void Fire_Double_Default() {
		nextFire = Time.time + (fireRate / weaponMagnitude * 1.5f);
		Instantiate (shot, shotSpawner.position, shot.transform.rotation); 							// Primary shot origin point
		Instantiate (shot_double, shotSpawner_double.position, shot_double.transform.rotation); 	// Double Shot origin point
	}
	public void Fire_Laser_Ripple() {
		nextFire = Time.time + (fireRate/weaponMagnitude);
		Instantiate(shot_laser, shotSpawner.position, shot_laser.transform.rotation); 				// Laser Mode
	}



	/*
	public void FiringSystem() {
		// Fire weapon based on current type
		switch (attackType) {
		case "DOUBLE":
			// Double shot mode
			if (Input.GetButton ("Fire_Main") && Time.time > nextFire) {
				nextFire = Time.time + (fireRate/weaponMagnitude * 1.5f);
				Instantiate(shot, shotSpawner.position, shot.transform.rotation); 						// Primary shot origin point
				Instantiate(shot_double, shotSpawner_double.position, shot_double.transform.rotation); 	// Double Shot origin point
			}
			break;

		case "LASER":
			// For Ripple Laser
			if (Input.GetButton ("Fire_Main") && Time.time > nextFire) {
				nextFire = Time.time + (fireRate/weaponMagnitude);
				Instantiate(shot_laser, shotSpawner.position, shot_laser.transform.rotation); // Laser Mode
			}
			break;

		case "SHOT":
		default:
			// Basic shot
			if (Input.GetButton ("Fire_Main") && Time.time > nextFire) {
				nextFire = Time.time + fireRate;
				Instantiate(shot, shotSpawner.position, transform.rotation); // Primary shot (basic)
			}
			break;
		}
	}
	*/
	void checkPlayerStats() {
		shot = 				playerController.shot;
		shot_double = 		playerController.shot_double;
		shot_laser = 		playerController.shot_laser;

		fireRate = 			playerController.fireRate;

		attackType = 		playerController.attackType;
		missile_status = 	playerController.missile_status;
		weaponMagnitude = 	playerController.weaponMagnitude;
	}
}
