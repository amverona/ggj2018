using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour {

	public GameObject fireballPrefab;
	public GameObject burstPrefab;

	public BodyParts body;

	public void ShootFireball() {
		GameObject instance = GameObject.Instantiate(fireballPrefab, body.handLBone.position, transform.rotation);
	}

	public void ShootBurst() {
		GameObject instance = GameObject.Instantiate(burstPrefab, body.thoraxBone.position, Quaternion.identity);
	}
}
