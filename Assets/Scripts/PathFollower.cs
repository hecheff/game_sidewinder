using UnityEngine;
using System.Collections;

public class PathFollower : MonoBehaviour {

	public Transform[] path;
	public float travelSpeed = 5.0f;	// Travel speed variable for objects following path
	public float reachDist = 1.0f;		// 
	public int currentPoint = 0;		// Set initial starting point

	void Start (){
		
	}

	void Update (){
		Vector3 dir = path [currentPoint].position - transform.position;
		transform.position += dir * Time.deltaTime * travelSpeed;

		if (dir.magnitude <= reachDist) {
			currentPoint++;
		}

		if (currentPoint >= path.Length) {
			currentPoint = 0;
		}
		//OnDrawGizmos ();
	}

	void OnDrawGizmos (){
		if (path.Length > 0) {
			for (int i = 0; i < path.Length; i++) {
				if (path [i] != null) {
					Gizmos.DrawSphere (path [i].position, reachDist);
				}
			}
		}
	}
}
