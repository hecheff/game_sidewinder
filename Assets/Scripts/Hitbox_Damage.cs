// Ship destroyed upon enemy/obstacle entering the hitbox

using UnityEngine;
using System.Collections;

public class Hitbox_Damage : MonoBehaviour {
	public PlayerController playerController; 	// Call instance of existing player controller
	public GameController gameController;		// Call instance of game controller for player lives
	public AudioClip destroySFX;

	public GameObject playerUnit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col){
		// If either an enemy, enemy attack, or obstacle collides with the player damage hitbox
		if (col.CompareTag("enemy")) {
			AudioSource.PlayClipAtPoint (destroySFX, transform.position);

			AudioSource audioSource = gameController.GetComponent<AudioSource> ();
			audioSource.Pause ();

			playerUnit.SetActive(false);	// Destroy player

			// Play sound effect destroyed
			// Play explosion animation

			// --- Check for remaining lives ---
			// If remaining lives = 0, check for continues available
			//		* If number of continues = 0, GAME OVER screen
			//		* Otherwise, show Continue Screen
			//			* If player chooses Continue, 
			//			* Otherwise, GAME OVER screen

			// Otherwise, remaining lives - 1, respawn player
		}




	}
}