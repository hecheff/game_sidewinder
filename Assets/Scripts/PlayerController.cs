using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

[System.Serializable]
public class Boundary {
	public float xMin, xMax, yMin, yMax;
}

public class PlayerController : MonoBehaviour {
	public float speed;
	public float tilt;
	public Boundary boundary;
	// public int hitPoint = 3;

	public GameObject shieldType;			// Shield Type being used
	public bool shieldStatus = false;		// Determines if Shield is currently active (if active, prevents Power Meter Shield from being accessible)

	public bool autoFire = false;
	public GameObject shot;
	public GameObject shot_double;
	public GameObject shot_laser;
	public Transform shotSpawner;			// Main shot spawner
	public Transform shotSpawner_double;	// Double Gun

	public float fireRate;
	private float nextFire = 0;

	// Sound effects
	//public AudioClip spawnSFX;			// Player spawning
	public AudioClip shotSFX;			// Weapon Shot (Basic)
	public AudioClip shotSFX_double;	// Weapon Shot (Double)
	public AudioClip shotSFX_laser;		// Weapon Shot (Laser)
	public AudioClip powerUpSFX;		// Using a power up

	Animator anim;

	private GameController gameController;
	public string attackType = "SHOT";		// Flag for current weapon type: SHOT | DOUBLE | LASER

	int currentSpeedUp = 0;
	public int speedUpLimit = 6;			// Upper limit for speed ups

	public int missile_status = 0;			// Check whether Missile is active
	public int weaponMagnitude = 1;			// Current weapon level
	public int optionMaxLimit;				// Max. number of Option attack drones

	public int optionCount = 0;				// Current number of Option attack drones 


	Camera camera;	// Camera variable used for determining pixelspace on screen

	// Variables for attracting power ups to player unit
	public float attractRadius, attractMagnitude;

	// Establish record of character movement coordinates (used by Options and other similar power-ups)
	//public Vector3[] coordinatesArray = new Vector3[2000];

	// Use this for initialization
	void Start () { 
		camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

		// Establish initial steps
		//for (int i = 0; i < 1; i++) { coordinatesArray[i] = GetComponent<Rigidbody> ().position; }

		//AudioSource.PlayClipAtPoint (spawnSFX, transform.position);
		anim = GetComponent<Animator> ();

		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController>();
		}
		if (gameController == null) { Debug.Log ("Cannot find 'GameController' script."); }

		// Synch position of Options
		GameObject.FindGameObjectWithTag("OptionGroup").GetComponent<Rigidbody>().transform.position = transform.position;

		CheckShield();
	}

	void Update() {
		
	}

	void FixedUpdate () {
		switch (gameController.inputMode) {
			case "TOUCH_HOMING":
				Control_TouchHoming ();
				break;

			case "TOUCH_PAD":
				MovementSystem_Touch ();
				break;

			case "TOUCH_DRAG":
				MovementSystem_TouchDrag ();
				break;

			case "DEFAULT":
			default:
				MovementSystem ();
				break;
		}

		FiringSystem ();	// Constantly check firing function
		FiringSystem2 ();	// Burst Attack
		// Debug.Log(CrossPlatformInputManager.GetButton ("Button_Reset") + " | " + CrossPlatformInputManager.GetButton ("Button_PowerUp"));
	}

	/*
	 * ---------------------------
	 * Touchscreen Functionality
	 * ---------------------------
	 * 
	 * Method 1: On-screen D-pad/Analog Pad + Buttons [DEFAULT]
	 * Parts:
	 * 		- On-screen Analog Pad (with distance sensitivity)
	 * 		- Shoot button
	 * 		- Powerup Button
	 * 		- Option Control Button
	 * 		- Pause
	 * 
	 * Functions: 
	 * 		- Left half of screen spawns analog pad wherever player touches (allowing player to 'flick' small movements)
	 * 			- May cause precision issues if analog pad stays static
	 * 			- Alternative: D-Pad is static
	 * 		- Right half of screen has pre-set placement of key inputs (Fire, Power Up, Option Control)
	 * 		- All button placings and sizes can be adjusted by the player to match their play style and device (useful for larger devices)
	 * 
	 * 
	 * Option Settings:
	 * 		- Input Config		TOUCH SCREEN | (Device Name)	Get full list of supported devices currently connected to device.
	 * 															Note: Any input device can be used simultaneously, this is for custom settings such as key config and moving the touchscreen UI buttons.
	 * 		- Autofire 			OFF | ON TOUCH | ALWAYS ON		Default ON TOUCH for Mobile Version
	 * 		- Gesture Powerup	OFF | ON						Tapping screen with two fingers will activate power up, allowing one-handed play
	 * 		- Power Ups			AUTO | MANUAL					Place in "Settings" menu, or when selecting power up modes before game begins?
	 * 
	 * Notes:
	 * 		- Finger Drag method impractical for this game due to Speed restrictions of player unit
	 * 			- Also not advised due to excessive screen obstruction by player's hand and fingers
	 * 		- Provide autodetect (or just simultaneous operation) if controller/keyboard detected connected to device
	 * 		- 
	*/


	private Vector3 target;
	private Vector3 ship_currLocation;
	private float 	delta_x;
	private float 	delta_y;

	private Vector3 destination;
	private float magnitude_x;
	private float magnitude_y;

	void Control_TouchHoming ()	{
		/*
		 * Homing feature
		 * ----------------
		 * 1. User touches any part of screen (not counting button (first finger only)
		 * 2. Unit moves towards player's finger at its preset max. speed
		 * 3. When no finger is detected, unit stops all movement
		*/

		if (Input.GetMouseButton (0)) {
			target = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));	// Convert mouse click point to game world (allows scaling)

			ship_currLocation = new Vector3 (GetComponent<Rigidbody> ().transform.position.x, GetComponent<Rigidbody> ().transform.position.y, 0.0f);

			delta_x = target.x - ship_currLocation.x;
			delta_y = target.y - ship_currLocation.y;

			destination = new Vector3 (delta_x, delta_y, 0.0f).normalized;
			Debug.Log(destination);
			GetComponent<Rigidbody> ().velocity = destination * speed;
			MovementTilt ();
		} else { GetComponent<Rigidbody> ().velocity = Vector3.zero; MovementTilt (); }
	}

	private float mouse_delta_x;
	private float mouse_delta_y;
	GameObject guider;
	private float drag_sensitivity = 2.0f;
	void MovementSystem_TouchDrag () {
		/*
			Basic Actions
			1. Upon touch on-screen:
				- Spawn "to-follow" object right on top of player (invisible)
			2. On drag, move "to-follow" object in exact pixelspace as drag
				- Player follows object at its current max.speed.
			3. Disable "to-follow" object upon release (prevents player unit from continuing to destination)
		*/

		// Set origin point of target object upon mouse click / touch
		guider = GameObject.Find ("Guider");
		if (Input.GetMouseButtonDown (0)) {
			guider.transform.position = GetComponent<Rigidbody> ().transform.position;

			// Set distance differences between touch and unit as pivot
			mouse_delta_x = guider.transform.position.x - camera.ScreenToWorldPoint (Input.mousePosition).x;
			mouse_delta_y = guider.transform.position.y - camera.ScreenToWorldPoint (Input.mousePosition).y;
		}

		// While touch is still down, 
		if (Input.GetMouseButton (0)) {
			// Player Unit follows guider
			guider.transform.position = new Vector3 (camera.ScreenToWorldPoint (Input.mousePosition).x + mouse_delta_x, camera.ScreenToWorldPoint (Input.mousePosition).y + mouse_delta_y, 0); 

			delta_x = guider.transform.position.x - GetComponent<Rigidbody> ().transform.position.x;
			delta_y = guider.transform.position.y - GetComponent<Rigidbody> ().transform.position.y;
			destination = new Vector3 (delta_x, delta_y, 0.0f).normalized;

			GetComponent<Rigidbody> ().velocity = destination * speed;
			MovementTilt ();
		} else {
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			MovementTilt ();
			guider.transform.position = GetComponent<Rigidbody> ().transform.position;
		}
	}


	// Input controller for tactile input types (keyboard, mouse, gamepads)
	void MovementSystem() {
		float moveHorizontal = 	Input.GetAxis ("Horizontal"); 
		float moveVertical = 	Input.GetAxis ("Vertical");
		Vector3 movement = new Vector3 (moveHorizontal, moveVertical, 0.0f);
		GetComponent<Rigidbody>().velocity = movement * speed;
		MovementTilt ();
	}

	void MovementSystem_Touch() {
		Vector2 moveVec = new Vector2 (CrossPlatformInputManager.GetAxis ("Horizontal"),CrossPlatformInputManager.GetAxis ("Vertical")) * speed;
		GetComponent<Rigidbody>().velocity = moveVec;
		MovementTilt ();
	}

	void MovementTilt() {
		GetComponent<Rigidbody>().position = 
			new Vector3(
				Mathf.Clamp(GetComponent<Rigidbody>().position.x,boundary.xMin,boundary.xMax), 
				Mathf.Clamp(GetComponent<Rigidbody>().position.y,boundary.yMin,boundary.yMax), 
				0.0f
			);
		GetComponent<Rigidbody>().rotation = Quaternion.Euler (GetComponent<Rigidbody>().velocity.y * tilt, 0.0f, 0.0f);
	}

	// Movement User Interface for Touchpad
	void Movement_UI() {
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
			GameObject.FindGameObjectWithTag ("UI_Input_Slidepad").SetActive(true);
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
		} else {
			GameObject.FindGameObjectWithTag ("UI_Input_Slidepad").SetActive(false);
		}
	}

	// Weapon firing mechanics
	public void FiringSystem () {
		// Fire weapon based on current type
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



	// -----------------------------------------------------------------------------------------------------------------------------
	// POWER UP FUNCTIONS


	public void Fire_Shot_Default() {
		nextFire = Time.time + fireRate;
		Instantiate(shot, shotSpawner.position, transform.rotation); 								// Primary shot (basic)
		AudioSource.PlayClipAtPoint (shotSFX, transform.position);
	}
	public void Fire_Double_Default() {
		nextFire = Time.time + (fireRate / weaponMagnitude * 1.5f);
		Instantiate (shot, shotSpawner.position, shot.transform.rotation); 							// Primary shot origin point
		Instantiate (shot_double, shotSpawner_double.position, shot_double.transform.rotation); 	// Double Shot origin point
		AudioSource.PlayClipAtPoint (shotSFX, transform.position);
	}
	public void Fire_Laser_Ripple() {
		nextFire = Time.time + (fireRate/weaponMagnitude);
		Instantiate(shot_laser, shotSpawner.position, shot_laser.transform.rotation); 				// Laser Mode
		AudioSource.PlayClipAtPoint (shotSFX_laser, transform.position);
	}


	// -----------------------------------------------------------------------------------------------------------------------------



	private int flag_powerUp;	// Flag for determining whether to execute power up: 0 = No Upgrade, 1 = Upgrade
	void FiringSystem2 ()
	{
		// Check for activation of power meter
		//CrossPlatformInputManager.GetButton("Button_PowerUp");

		flag_powerUp = 1; // Set enabled as default
		if ((CrossPlatformInputManager.GetButton ("PowerUp") || (Input.GetButton ("PowerUp"))) && gameController.powerLevel != 0) {
			// Check current power level which triggers respective power-up category
			// Implement power-up check on current player configuration (encapsulated conditional triggers)
			switch (gameController.powerLevel) {
			case 1:
				// SPEED UP
				/* 
				 * Increases max. speed of player unit
				 * Need to implement upper limit of speed, using one or combination of several methods:
				 * 		1. Speed Limit (a default max. speed of sorts)
				 * 		2. Current Speed = Base Speed * 
				*/
				if (currentSpeedUp < speedUpLimit) {
					speed += 3;
					currentSpeedUp++;
				} else {
					flag_powerUp = 0;
				}
				break;
			case 2:
				// MISSILE
				/*
				 * Activates Missile subweapon of player unit.
				 * 
				*/
				/*if (missile_status == 0) {
					missile_status = 1;	// Enable missile weapons
				} else if (missile_status == 1) {
					missile_status = 2;
				}*/

				flag_powerUp = 0;
				break;
			case 3:
				// DOUBLE
				/*
				 * Replaces normal weapon with double gun
				 * 		1. Front + Diagonal
				 * 		2. Front + Up
				 * 		3. Front + Back
				 * 		4. Double Front
				*/
				if (attackType != "DOUBLE") {
					attackType = "DOUBLE";
					weaponMagnitude = 1;
				} else if (attackType == "DOUBLE") {
					if (weaponMagnitude == 1) {
						weaponMagnitude = 2;
					} else {
						// Do nothing. Level 2 Max.
						flag_powerUp = 0;
					}
				}
				break;
			case 4:
				// LASER
				/*
				 * Replaces normal weapon with laser 
				 * 		1. Long beam which follows vertical position of machine (can be used like a crude blade for sweeping)
				 * 		2. 
				*/
				if (attackType != "LASER") {
					attackType = "LASER";
					weaponMagnitude = 1;
				} else if (attackType == "LASER") {
					if (weaponMagnitude == 1) {
						weaponMagnitude = 2;
					} else {
						// Do nothing. Level 2 Max.
						flag_powerUp = 0;
					}
				}
				break;
			case 5:
				// OPTION
				/* 
				 * Spawns an Option Drone. Can be repeated up to a preset limit (usually 4 for basic types)
				 * Various types:
				 * 		1. Basic: Fires forward duplicating your primary weapon and missile, firing in synch with you
				 * 		2. 
				*/
				if (optionCount < optionMaxLimit) {
					// Spawn Option ball on/behind Player
					optionCount++;
				} else {
					flag_powerUp = 0;
				}
				break;
			case 6:
				// (?) SHIELD
				/* 
				 * Activates player unit's shield system (type depending on ship, can be customized)
				*/
				if (shieldStatus == false) {
					shieldStatus = true;
					Vector3 playermodel = GameObject.Find("vicviper").transform.position;
					GameObject newShield = Instantiate(shieldType, playermodel, Quaternion.identity) as GameObject;
					newShield.transform.parent = GameObject.Find("vicviper").transform;
				} else {
					flag_powerUp = 0;
				}
				break;
			case 7:
				// (!) EX
				/* 
				 * Unleashes an unique ability based on the base chassis of the machine. Unique to each unit's base chassis
				*/
				flag_powerUp = 0;
				break;
			}
			if (flag_powerUp == 1) {
				gameController.powerLevel = 0;
				gameController.UpdatePowerLv_2 (0);
				AudioSource.PlayClipAtPoint (powerUpSFX, transform.position);
			}
		}
	}

	// Actions when objects enter player hitbox
	void OnTriggerEnter(Collider other) {
		// Identify collider in effect
		if (other.CompareTag ("Boundary")) {
			// Do nothing
		} else {
			//Debug.Log (GetComponentInChildren<Collider> ());
			if (GetComponentInChildren<Collider> ().CompareTag ("hitbox_player_item") == true) { 
				// Items
				//Debug.Log (GetComponentInChildren<Collider> ());
				if (other.gameObject.CompareTag ("powerup_1_bronze") || other.gameObject.CompareTag ("powerup_2_silver") || other.gameObject.CompareTag ("powerup_3_gold")) {
					if (other.gameObject.CompareTag ("powerup_1_bronze") == true) {
						// Debug.Log ("1");
					}
					/*
					if (other.gameObject.CompareTag ("powerup_2_silver") == true) {
						Debug.Log ("2");
					}
					if (other.gameObject.CompareTag ("powerup_3_gold") == true) {
						Debug.Log ("5");
					}
					*/
					//other.gameObject.SetActive (false);
				}
			} 
			/*
			else if (GetComponentInChildren<Collider> ().CompareTag ("hitbox_player_damage") == true) { 
				// Enemies
				if (other.gameObject.CompareTag ("enemy")) {
					Debug.Log ("Damage taken from enemy contact");
				}
				Debug.Log ("Damage taken from enemy contact");
				GetComponent<Animator>().SetTrigger ("PlayerDie");
			}*/
		}
	}
	void State_Idle() {
		anim.enabled = false;
	}

	void CheckShield() {
		if (shieldStatus == true) {
			
			//shieldType.SetActive(true);
			//GameObject.FindGameObjectWithTag("player_forceShield").GetComponent<BoxCollider>().enabled = true;
		} else {
			
			//shieldType.SetActive(false);
			//GameObject.FindGameObjectWithTag("player_forceShield").GetComponent<BoxCollider>().enabled = false;
		}
	}

	/*
	void MovementPath() {
		
		// Updates list ONLY when there is movement (to prevent Options from stacking up on player
		if (Input.GetAxis ("Horizontal") != 0.0f || Input.GetAxis ("Vertical") != 0.0f) {
			// For initial, just set coordinates to current player coordinates (no updating of array)
			if (coordinatesArray [0] == null) { 
				// Populate sufficient padding
				for (int i = 0; i < 1; i++) {
					coordinatesArray [0] = GetComponent<Rigidbody> ().position;
				}
			} else {
				for (int i = 0; i < coordinatesArray.Length - 1; i++) {
					coordinatesArray [i + 1] = coordinatesArray [i];
				}
				coordinatesArray [0] = GetComponent<Rigidbody> ().position; 
			}
			Debug.Log (coordinatesArray[0]);
		}

	}
	*/

	// Power-up Attract System
	// Deprecated due to lack of need with separated hitboxes, but may be reserved for later
	/*
	void ObjectPull() {
		foreach (Collider collider in Physics.OverlapSphere(transform.position, attractRadius)) {
			if (collider.gameObject.CompareTag ("powerup_1_bronze") || collider.gameObject.CompareTag ("powerup_2_silver") || collider.gameObject.CompareTag ("powerup_3_gold")) {
				Vector3 forceDirection = transform.position - collider.transform.position;

				collider.GetComponent<Rigidbody> ().AddForce (forceDirection.normalized * attractMagnitude);
				collider.transform.localScale -= Vector3.one*Time.deltaTime*0.1f;

				//collider.GetComponent<Rigidbody> ().AddForce (forceDirection.normalized * attractMagnitude * Time.fixedDeltaTime);
				//GetComponent<Rigidbody>().AddForce(forceDirection.normalized * attractMagnitude * Time.fixedDeltaTime);
				//collider.attachedRigidbody.AddForce(forceDirection.normalized * attractMagnitude * Time.fixedDeltaTime);
			}
		}
	}
	*/
}