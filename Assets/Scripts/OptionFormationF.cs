using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class OptionFormationF : MonoBehaviour {
	GameObject player;
	private PlayerController playerController;
	private GameController gameController;

	public GameObject[] option;
	
	void Start () {
		// Get all behaviors from player controller
		player = GameObject.Find("Player");
		if (player != null) {
			playerController = player.GetComponent<PlayerController>();
		} if (player == null) { Debug.Log ("Cannot find 'PlayerController' script."); }
		checkOptionCount ();
	}
	
	// Update is called once per frame
	void Update () {
		checkOptionCount ();
	}

	void FixedUpdate() {
		checkOptionCount ();

		// If player is active, enable controls. Otherwise do nothing
		if (player.activeSelf != false) {
			OptionGroup_F_update2 ();
			/*
			OptionGroup_F_update ();
			OptionGroup_F_update_touch ();
			*/
		} else {
			// Do nothing as player is currently in "Destroyed" state
			playerController.optionCount = 0;	// Player has no "Options" when destroyed
		}
	}
	/*
	public void OptionGroup_F_initial() {
		GetComponent<Rigidbody>().transform.position = playerController.transform.position;
	}
	*/
	public void OptionGroup_F_update2() {
		GetComponent<Rigidbody> ().transform.position = playerController.transform.position;
	}
	public void checkOptionCount() {
		for (int i = 0; i < playerController.optionCount; i++) {
			option [i].SetActive (true);
		}
		for (int i2 = playerController.optionCount; i2 < playerController.optionMaxLimit; i2++) {
			option [i2].SetActive (false);
		}
	}

	/*
	public void OptionGroup_F_update() {
		float moveHorizontal = 	Input.GetAxis ("Horizontal");
		float moveVertical = 	Input.GetAxis ("Vertical");
		Vector3 playerCoordinates 	= playerController.transform.position;
		//Vector3 dist_delta = playerCoordinates - transform.position;

		if ((Input.GetAxis ("Horizontal") != 0.0f || Input.GetAxis ("Vertical") != 0.0f)) {
			Vector3 movement = new Vector3 (moveHorizontal, moveVertical, 0.0f);
			GetComponent<Rigidbody> ().velocity = movement * playerController.speed;
			GetComponent<Rigidbody> ().transform.position = 
				new Vector3 (
					Mathf.Clamp (GetComponent<Rigidbody> ().position.x, playerController.boundary.xMin, playerController.boundary.xMax), 
					Mathf.Clamp (GetComponent<Rigidbody> ().position.y, playerController.boundary.yMin, playerController.boundary.yMax), 
					0.0f
				);
		} else {
			GetComponent<Rigidbody> ().velocity = new Vector3(0.0f,0.0f,0.0f);
		}
	}
	public void OptionGroup_F_update_touch() {
		Vector2 moveVec = new Vector2 (CrossPlatformInputManager.GetAxis ("Horizontal"),CrossPlatformInputManager.GetAxis ("Vertical"));
		Vector3 playerCoordinates 	= playerController.transform.position;
		//Vector3 dist_delta = playerCoordinates - transform.position;

		if ((CrossPlatformInputManager.GetAxis ("Horizontal") != 0.0f || CrossPlatformInputManager.GetAxis ("Vertical") != 0.0f)) {
			GetComponent<Rigidbody> ().velocity = moveVec * playerController.speed;
			GetComponent<Rigidbody> ().transform.position = 
				new Vector3 (
					Mathf.Clamp (GetComponent<Rigidbody> ().position.x, playerController.boundary.xMin, playerController.boundary.xMax), 
					Mathf.Clamp (GetComponent<Rigidbody> ().position.y, playerController.boundary.yMin, playerController.boundary.yMax), 
					0.0f
				);
		} else {
			GetComponent<Rigidbody> ().velocity = new Vector3(0.0f,0.0f,0.0f);
		}
	}
	*/
}