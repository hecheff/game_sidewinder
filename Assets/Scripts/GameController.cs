using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class GameController : MonoBehaviour {

	public string inputMode = "TOUCH_HOMING";	// Input Mode Sets		DEFAULT | NO_TOUCH | TOUCH

	public AudioClip pauseFX;		// Pause Sound Effect
	public GUIText Power_Marker;	// Power Meter Marker
	public int powerLevel;			// Current Power level

	// Use this for initialization
	void Start () {
		powerLevel = 0;

		// Set initial set of Power Level
		//UpdatePowerLv (powerLevel);	
		UpdatePowerLv_2 (powerLevel);

		SetGamepad ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButton ("Pause")) {
			Debug.Log ("PAUSE!");
			Pause ();
		}

		//CrossPlatformInputManager.GetButton("Reset");
		if (CrossPlatformInputManager.GetButtonDown ("Reset") == true){
			Debug.Log("Reset button pressed!");
			Application.LoadLevel (Application.loadedLevel);
		}
	}

	void FixedUpdate() {
		
	}

	public void SetInputs() {
		switch (inputMode) {
			case "NO_TOUCH":
				GameObject.Find ("_UI_TOUCH").SetActive (false);
				GameObject.Find("Guider").SetActive(false);
				break;

			case "TOUCH_DRAG":
				GameObject.Find("Guider").SetActive(true);
				break;
			
			case "TOUCH_HOMING":
				GameObject.Find("Guider").SetActive(false);
				break;

			case "DEFAULT":
			default:
				GameObject.Find("Guider").SetActive(false);
				break;
		}
	}

	bool isPaused = false;
	public void Pause() {
		if (isPaused == true){
			Time.timeScale = 1;
			isPaused = false;
		} else {
			Time.timeScale = 0;
			isPaused = true;
		}
		AudioSource.PlayClipAtPoint (pauseFX, transform.position);
	}

	void SetGamepad() {
		// Check: Only enable touchpad when input mode is set to "TOUCH_PAD"
		if (inputMode == "TOUCH_PAD") {
			GameObject.FindGameObjectWithTag("UI_Input_Slidepad").SetActive(true);
		} else { GameObject.FindGameObjectWithTag("UI_Input_Slidepad").SetActive(false); }
	}

	public void AddPowerLv(int powerUpLv) {
		if (powerLevel + powerUpLv > 7) {
			powerLevel = 1;
		} else {
			powerLevel += powerUpLv;
		}
		//UpdatePowerLv(powerLevel);
		UpdatePowerLv_2(powerLevel);
	}

	public void UpdatePowerLv(int powerLevel) {
		// Calculate Tier Level Here
		/*
		if (powerLevel >= 0 && powerLevel < 10) {
			powerTier = "1";
		} else if (powerLevel >= 10 && powerLevel < 20) {
			powerTier = "2";
		} else if (powerLevel >= 20 && powerLevel < 30) {
			powerTier = "3";
		} else if (powerLevel == 30) {
			powerTier = "MAX";
		}
		*/

		float marker_xpos = 0.978f;

		switch (powerLevel) {
			case 0: Power_Marker.GetComponent<GUIText> ().transform.position = new Vector3(marker_xpos,-1,0); 		break;
			case 1: Power_Marker.GetComponent<GUIText> ().transform.position = new Vector3(marker_xpos,0.96f,0); 	break;
			case 2: Power_Marker.GetComponent<GUIText> ().transform.position = new Vector3(marker_xpos,0.93f,0); 	break;
			case 3: Power_Marker.GetComponent<GUIText> ().transform.position = new Vector3(marker_xpos,0.90f,0);	break;
			case 4: Power_Marker.GetComponent<GUIText> ().transform.position = new Vector3(marker_xpos,0.87f,0); 	break;
			case 5: Power_Marker.GetComponent<GUIText> ().transform.position = new Vector3(marker_xpos,0.84f,0);	break;
			case 6: Power_Marker.GetComponent<GUIText> ().transform.position = new Vector3(marker_xpos,0.81f,0); 	break;
			case 7: Power_Marker.GetComponent<GUIText> ().transform.position = new Vector3(marker_xpos,0.78f,0);	break;
		}
	}

	public void UpdatePowerLv_2 (int powerLevel) {
		GameObject icon = GameObject.Find("meter_marker");
		GameObject pow_01 = GameObject.Find("pow_01");
		GameObject pow_02 = GameObject.Find("pow_02");
		GameObject pow_03 = GameObject.Find("pow_03");
		GameObject pow_04 = GameObject.Find("pow_04");
		GameObject pow_05 = GameObject.Find("pow_05");
		GameObject pow_06 = GameObject.Find("pow_06");
		GameObject pow_07 = GameObject.Find("pow_07");

		switch (powerLevel) {
			case 0: icon.transform.position = new Vector3(-999,-999,0);		break;
			case 1: icon.transform.position = pow_01.transform.position;	break;
			case 2: icon.transform.position = pow_02.transform.position;	break;
			case 3: icon.transform.position = pow_03.transform.position;	break;
			case 4: icon.transform.position = pow_04.transform.position;	break;
			case 5: icon.transform.position = pow_05.transform.position;	break;
			case 6: icon.transform.position = pow_06.transform.position;	break;
			case 7: icon.transform.position = pow_07.transform.position;	break;
		}
	}



	/*
	public void AddPowerLv(int powerUpLv) {
		if (powerLevel + powerUpLv >= 30) {
			powerLevel = 30;
		} else {
			powerLevel += powerUpLv;
		}
		UpdatePowerLv();
	}

	void UpdatePowerLv() {
		// Calculate Tier Level Here
		if (powerLevel >= 0 && powerLevel < 10) {
			powerTier = "1";
		} else if (powerLevel >= 10 && powerLevel < 20) {
			powerTier = "2";
		} else if (powerLevel >= 20 && powerLevel < 30) {
			powerTier = "3";
		} else if (powerLevel == 30) {
			powerTier = "MAX";
		}


		PowerLevelText.text = "[" + powerTier + "] Power Lv: " + powerLevel;

	}
	*/

	public void ResetGame () {
		//Time.time = 0;
		Application.LoadLevel (Application.loadedLevel);
	}
}