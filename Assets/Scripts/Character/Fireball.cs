using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {
	//public SphereCollider sphereCollider;
	public ParticleSystem particleSys;
	public Light pointLight;
	public float pointLightScale = 1f;
	public Transform mesh;
	public float meshScale = 0.2f;
	public Rigidbody rbody;
	
	public float time = 4f;
	public AnimationCurve curve;

	private float lifeTime = -1f;

	public float forwardSpeed = 5f;

	public void Awake() {
		pointLight.range = 0f;
		particleSys.Stop();
		mesh.localScale = Vector3.zero;
		rbody.Sleep();
	}

	public void Start() {
		Go();
	}

	public void Update() {
		if(lifeTime < 0f)
			return;

		if(lifeTime < Time.time) {
			particleSys.transform.SetParent(null);
			particleSys.Stop();

			Destroy(particleSys.gameObject, particleSys.main.duration);
			Destroy(gameObject);
			
			return;
		}

		float evalTime = curve.Evaluate((lifeTime - Time.time)/time);

		pointLight.range = pointLightScale * evalTime;
		mesh.localScale = Vector3.one * meshScale * evalTime;
		
		transform.position += transform.forward * forwardSpeed * Time.deltaTime;
	}

	public void Go() {
		lifeTime = Time.time + time;
		particleSys.Play();
	}
}
