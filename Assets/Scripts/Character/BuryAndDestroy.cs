using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuryAndDestroy : MonoBehaviour {
	private Collider[] colliders;

	public float waitInterval;
	public float fallInterval;

	private float waitTime = -1f;
	private float fallTime = -1f;

	public void Start() {
		colliders = transform.GetComponentsInChildren<Collider>();

		waitTime = Time.time + waitInterval;		
	}

	public void Update() {
		if(waitTime > Time.time) {
			return;
		} else {
			if(fallTime > Time.time) {
				return;
			} else if(fallTime >= 0f) {
				Destroy(gameObject);
			}

			Bury();

			fallTime = Time.time + fallInterval;
		}
	}

	private void Bury() {
		foreach(Collider collider in colliders) {
			collider.enabled = false;
		}
	}
}
