using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackObject : MonoBehaviour {

	public Transform target;

	public bool position;
	public bool rotation;
	public bool scale;

	void LateUpdate () {
		if(position) transform.position = target.position;
		if(rotation) transform.rotation = target.rotation;
		if(scale) transform.localScale = target.localScale;
	}
}
