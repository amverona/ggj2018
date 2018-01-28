using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationArea : MonoBehaviour {

	private Transform parent;
	private SphereCollider sphere;

	private float baseRadius;

	public void Start() {
		parent = transform.parent;

		sphere = GetComponent<SphereCollider>();

		baseRadius = sphere.radius;
		
		Hide();
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

		parent.gameObject.SetActive(true);
		transform.SetParent(parent);

		sphere.radius = baseRadius * 1.1f;
	}

	private void Hide() {
		if(parent == null) {
			Destroy(gameObject);
			return;
		}
		
		parent.gameObject.SetActive(false);
		transform.SetParent(null);
		parent.SetParent(transform);

		sphere.radius = baseRadius;
	}
}
