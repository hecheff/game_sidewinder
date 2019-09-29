// Remove object from game if it leaves the boundary

/* 
	* Need to implement feature which allows destruction of object IF AND ONLY IF leaving boundary (allowing entry spawns)
*/
using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour {
	/*
	void OnTriggerExit(Collider other) {
		Destroy (other.gameObject);
	}
	*/

	void OnTriggerEnter(Collider other) {
		// Do nothing
		//Debug.Log("ENTRY");
	}
	void OnTriggerExit(Collider other) {
		if (other.CompareTag ("player_attack")) {
			Destroy (other.gameObject);
		} else if (other.CompareTag ("enemy")) {
			//other.gameObject.SetActive (false);
		}
	}
}