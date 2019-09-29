using UnityEngine;
using System.Collections;

public class PowerUpMagnet : MonoBehaviour {

	public float attractRadius, attractMagnitude;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ObjectPull ();
	}

	// Power-up Attract System
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
}