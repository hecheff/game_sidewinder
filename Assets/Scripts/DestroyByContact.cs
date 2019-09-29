using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {
	// POWER UP VARIABLES
	public int powerLevel = 0;	// Amount added to Power Meter. Leave 0 if not a power up.

	// EVEMY VARIABLES
	public int enemyHP = 1;		// Enemy Hit Points
	public Shader hitShader;

	private GameController gameController;


	public GameObject disappearVFX;	// Visual effect which plays upon object removal from game (e.g. explosion)
	public AudioClip disappearSFX;	// Sound effect which plays upon object removal from game
	public AudioClip damageSFX;		// Sound effect which plays upon object taking damage

	void Start() {
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController>();
		}
		if (gameController == null) { Debug.Log ("Cannot find 'GameController' script."); }
	}

	void OnTriggerEnter (Collider other)
	{
		// Function differently between Power Ups and enemies (Player has own functions)
		if (gameObject.CompareTag ("powerup_1_bronze") || gameObject.CompareTag ("powerup_2_silver") || gameObject.CompareTag ("powerup_3_gold")) {
			// Add to power gauge if player's power-up hitbox. Otherwise do nothing.
			if (other.gameObject.CompareTag ("hitbox_player_item") == true) {
				Destroy (gameObject);
				gameController.AddPowerLv (powerLevel);
				AudioSource.PlayClipAtPoint (disappearSFX, transform.position);
			}
		} else if (gameObject.CompareTag ("enemy")) {
			if (other.gameObject.CompareTag ("player_attack") == true || other.gameObject.CompareTag ("player_forceShield") == true) {
				if (other.gameObject.CompareTag ("player_attack") == true) {
					Destroy (other.gameObject);
				}
				enemyHP--;
				
				if (enemyHP == 0) {
					// If enemy HP is 0, remove from game and play sound clip
					Destroy (gameObject);
					AudioSource.PlayClipAtPoint (disappearSFX, transform.position);		// Add explosion FX
				} else {
					// In the event enemy is not defeated
					// Debug.Log ("Enemy HP: " + enemyHP);
					if (damageSFX != null) {
						AudioSource.PlayClipAtPoint (damageSFX, transform.position);		// Add damage-taken FX
					}
					// gameObject.GetComponent<Animator>().SetTrigger("enemy_damageTaken");
					// Visual hit indication: Make enemy blink white once, revert.
				}
			}
			/*
			else if (gameObject.CompareTag ("player")) {
				if (other.gameObject.CompareTag ("enemy_attack")) {
					Destroy (gameObject);
				}
			}
			*/
		} else if (gameObject.CompareTag ("player_attack")) {
			if (other.gameObject.CompareTag ("player_attack")) {
				Debug.Log ("Attack collision detected!");
			} 
		}

		/*
		if (other.gameObject.CompareTag ("hitbox_player_item") == true) {
			gameController.AddPowerLv (powerLevel);
		} else if (other.gameObject.CompareTag ("player_attack") == true) {
			Debug.Log ("Hit!");
			other.gameObject.SetActive (false);
		} else if (other.gameObject.CompareTag ("enemy") == true) {
			
		}
		*/
	}
}