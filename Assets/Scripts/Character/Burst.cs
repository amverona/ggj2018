using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burst : MonoBehaviour {
	public SphereCollider sphereCollider;
	public float sphereColliderScale = 1f;
	public ParticleSystem particleSys;
	public Light pointLight;
	public float pointLightScale = 1f;
	
	public float time = 2f;
	public AnimationCurve curve;

	private float explosionTime = -1f;

	public void Awake() {
		sphereCollider.radius = 0f;
		pointLight.range = 0f;
		particleSys.Stop();
	}

	public void Start() {
		Explode();
	}

	public void Update() {
		if(explosionTime < 0f)
			return;

		if(explosionTime < Time.time) {
			Destroy(gameObject);
			return;
		}

		float evalTime = curve.Evaluate((explosionTime - Time.time)/time);
		
		sphereCollider.radius = sphereColliderScale * evalTime;
		pointLight.range = pointLightScale * evalTime;
	}

	public void Explode() {
		explosionTime = Time.time + time;
		particleSys.Play();
	}
}
