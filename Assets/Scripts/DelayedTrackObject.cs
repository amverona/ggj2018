using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedTrackObject : TrackObject {

	public float speed = 1f;

	void LateUpdate () {
		if(target == null)
			return;
			
		if(position) transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
		if(rotation) transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * speed);
		if(scale) transform.localScale = Vector3.Lerp(transform.localScale, target.localScale, Time.deltaTime * speed);
	}
}
