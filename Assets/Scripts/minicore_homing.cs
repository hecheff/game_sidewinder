using UnityEngine;
using System.Collections;

public class minicore_homing : MonoBehaviour {

	public float movementSpeed 	= 1;
	public float rotateSpeed 	= 1;

	private Transform target;
	private Vector3 direction;

	// Use this for initialization
	void Start () {
		// Check if Player is still 'active' before determining homing
		// If not (i.e. in 'destroyed' state), enemy unit continues moving in current direction
		if (GameObject.FindGameObjectWithTag ("Player") != null) {
			target = GameObject.FindGameObjectWithTag ("Player").transform;	// Set target as in-game player
		} else {
			target = null;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (target) {
			transform.LookAt (target);
			transform.Rotate (0.0f, 0.0f, 90.0f);
			//GetComponent<Rigidbody>().rotation.z = 90.0f;
			/*
			direction = target.position - GetComponent<Rigidbody>().position;
			direction.Normalize();
			Quaternion currentDirection = Quaternion.LookRotation (direction);
			transform.rotation = Quaternion.Slerp (transform.rotation, currentDirection, rotateSpeed);
			*/

			/*
			Quaternion rot = transform.rotation;
			rot.z = 45;
			transform.rotation = rot;
			*/
		}

		//GetComponent<Rigidbody> ().AddForce (direction * movementSpeed);
		if (target) {
			direction = target.position - GetComponent<Rigidbody> ().position;
			direction.Normalize ();
			GetComponent<Rigidbody> ().velocity = (direction * movementSpeed);
		} else {

		}
	}

	void FixedUpdate() {
		
	}
}