using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackObject : MonoBehaviour {

	public Transform target;

	public bool position;
	public bool rotation;
	public bool scale;

	public bool lockYRot;	

	void LateUpdate () {
		if(target == null)
			return;

		if(position) transform.position = target.position;
		if(rotation) {
			if(lockYRot) {
				Quaternion rotAux = Quaternion.Euler(transform.rotation.eulerAngles.x, -target.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

				transform.rotation = rotAux;
			} else {
				transform.rotation = target.rotation;
			}
		}
		if(scale) transform.localScale = target.localScale;
	}
}
