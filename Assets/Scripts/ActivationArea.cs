using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationArea : MonoBehaviour {

	private Transform parent;
	private Transform rootParent;
	private SphereCollider sphere;

	private float baseRadius;

	public void Awake() {
		parent = transform.parent;

		sphere = GetComponent<SphereCollider>();

		baseRadius = sphere.radius;

		sphere.radius = 0f;
	}

	public void Start() {
		rootParent = parent.parent;

		Hide();

		sphere.radius = baseRadius;
	}

    public void OnTriggerEnter(Collider other) {
        GameObject otherGO = other.gameObject;

		if(otherGO.CompareTag("Player")) {
            Show();
        }
    }

	public void OnTriggerExit(Collider other) {
        GameObject otherGO = other.gameObject;

		if(otherGO.CompareTag("Player")) {
            Hide();
        }
    }

	private void Show() {
		if(parent == null) {
			Destroy(gameObject);
			return;
		}

		parent.SetParent(rootParent);
		transform.SetParent(parent);
		parent.gameObject.SetActive(true);

		sphere.radius = baseRadius * 1.1f;
	}

	private void Hide() {
		if(parent == null) {
			Destroy(gameObject);
			return;
		}
		
		transform.SetParent(rootParent);
		parent.SetParent(transform);
		parent.gameObject.SetActive(false);

		sphere.radius = baseRadius;
	}
}
