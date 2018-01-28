using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabingEnemy : MonoBehaviour {

	public GameObject ragdollPrefab;

    public Transform modelRoot;

    public void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Damage")) {
            Death();
        }
    }

	private void Death() {
		GameObject ragdollInstance = GameObject.Instantiate(ragdollPrefab, transform.position, transform.rotation);

		Utils.CopyTransformsRecurse(modelRoot, ragdollInstance.transform);

        BodyParts ragdollBodyParts = ragdollInstance.GetComponent<BodyParts>();

        ragdollBodyParts.head.SetActive(false);

		Destroy(gameObject);
	}
}
