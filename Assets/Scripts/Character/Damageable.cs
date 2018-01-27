using UnityEngine;
using System.Collections;

public class Damageable : MonoBehaviour {	
	public float health;
	public float maxHealth;
	
	public float healthLossFactor;

	public delegate void HitEvent(GameObject origin, Vector3 position, float strength);
	public event HitEvent OnHit;

	public virtual void Hit(GameObject origin, Vector3 position, float strength) {
		if (OnHit != null) {
			OnHit (origin, position, strength);
		}
	}
}
