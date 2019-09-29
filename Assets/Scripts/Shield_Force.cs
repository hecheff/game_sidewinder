using UnityEngine;
using System.Collections;

public class Shield_Force : MonoBehaviour {

	public int shieldDurability = 3;
	public AudioClip shield_hit;

	GameObject player;
	PlayerController playerController;

	// Use this for initialization
	void Start () {
		shieldDurability = 3;

		player = GameObject.Find("Player");
		if (player != null) {
			playerController = player.GetComponent<PlayerController>();
		} if (player == null) { Debug.Log ("Cannot find 'PlayerController' script."); }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		SetShieldStatus ();
	}

	void OnTriggerEnter (Collider other) {
		// Reduce shield durability by 1 on contact with: Enemy | Enemy Attack
		if (other.CompareTag ("enemy")) {
			// Does 1 damage to enemy (coded in DestroyByContact)
			// Reduce shield durability by 1
			shieldDurability--;
		}
		Debug.Log(shieldDurability);
	}

	void SetShieldStatus () {
		GameObject forceShield = GameObject.FindGameObjectWithTag("player_forceShield_sprite");
		switch (shieldDurability) {
			case 3:
				forceShield.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
				gameObject.SetActive(true);
				//gameObject.GetComponent<BoxCollider>().enabled = true;
				break;
			case 2: 
				forceShield.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.6f);
				gameObject.SetActive(true);
				//gameObject.GetComponent<BoxCollider>().enabled = true;
				break;
			case 1: 
				forceShield.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.3f);
				gameObject.SetActive(true);
				//gameObject.GetComponent<BoxCollider>().enabled = true;
				break;
			case 0: 
			default:
				playerController.shieldStatus = false;
				forceShield.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.0f);
				Destroy(gameObject);
				//gameObject.GetComponent<BoxCollider>().enabled = false;
				break;
		}
	}
}
