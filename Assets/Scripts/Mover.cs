using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	public float speed;
	void Start() {
		//GetComponent<Rigidbody>().velocity = transform.forward * (speed * Random.Range(1,3));
		GetComponent<Rigidbody>().velocity = transform.right * (speed);
	}
}