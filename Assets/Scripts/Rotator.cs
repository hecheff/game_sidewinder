using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	public string axis;
	public float angle_per_second;

	void Update() {
		switch(axis) {
			case "X":
				transform.Rotate (new Vector3 (angle_per_second, 0, 0) * Time.deltaTime);
				break;

			case "Y":
				transform.Rotate (new Vector3 (0, angle_per_second, 0) * Time.deltaTime);
				break;

			case "Z":
				transform.Rotate (new Vector3 (0, 0, angle_per_second) * Time.deltaTime);
				break;
		}
	}
}