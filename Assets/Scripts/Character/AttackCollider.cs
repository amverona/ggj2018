using UnityEngine;
using System.Collections;

public class AttackCollider : MonoBehaviour {

	public GameObject root;
	public float strength;

	public void OnTriggerEnter(Collider other) {
		if (other.gameObject == root)
			return;

		Damageable damageable = other.GetComponent<Damageable>();

		if(damageable != null) {
			damageable.Hit(root, transform.position, strength);
		}
	}
}
