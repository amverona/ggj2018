using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodySleep : MonoBehaviour {
	private Rigidbody[] rigidbodies;

	public float time = 1f;

	private float sleepTime = -1f;

	public void Awake() {
		rigidbodies = transform.GetComponentsInChildren<Rigidbody>();

		Sleep();
	}

	public void Start() {
		sleepTime = time + Time.time;
	}

	public void FixedUpdate() {
		if(sleepTime > Time.time) {
			Sleep();
		} else {
			WakeUp();
			enabled = false;
		}
	}

	private void Sleep() {
		foreach(Rigidbody rBody in rigidbodies) {
			rBody.Sleep();
		}
	}

	private void WakeUp() {
		foreach(Rigidbody rBody in rigidbodies) {
			rBody.isKinematic = false;
			rBody.WakeUp();
		}
	}
}
