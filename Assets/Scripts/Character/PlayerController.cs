using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Damageable {

	public CharacterController characterController;

	public Vector3 moveDirection;
	public Vector3 lookDirection;
	public float lookSpeed = 2f;
	public float moveSpeed = 0.1f;
	public float smoothMoveSpeed = 0.1f;
	public float runSpeed = 0.1f;
	public float rotSpeed = 0.1f;
	public float smoothRotSpeed = 0.1f;
	public Camera mainCamera;

	public Animator anim;

	private float hurtTime = -1f;

	private bool run;
		
	public float invincibleInterval = 0.5f;
	private float invincibleTime = -1f;

	public float attackInterval = 1f;
	private float attackFireballTime = -1f;
	private float attackBurstTime = -1f;

	public float cooldownInterval = 0.5f;
	private float cooldownTime = -1f;

	private Vector3 desPos;
	private bool useNavAgent;

	[SerializeField]
	public BodyParts body;

	public Transform theFlame;

	public GameObject ragdollPrefab;

	public float changeBodyInterval = 1f;

	private float changeBodyTime = -1f;
	private float lockInputTime = -1f;

	public float fireballHealthLoss = 0.1f;
	public float burstHealthLoss = 0.1f;

	private List<Transform> grabbers;

	public Transform modelRoot;
	
	private bool dead;
	
	//private UnityEngine.AI.NavMeshAgent navMeshAgent;

	void Awake() {
		grabbers = new List<Transform>();
	}

	void Start() {
		if (mainCamera == null) {
			mainCamera = Camera.main;
		}

		lookDirection = transform.forward;
	
		//flameTracker.target = body.headBone;
	}

	void Update () {
		if (hurtTime > Time.time) {
			run = false;
			moveDirection = Vector3.zero;
		} else if(lockInputTime > Time.time) {
			useNavAgent = false;
		} else {
			UpdateInput ();
		}

		if (useNavAgent && (Vector3.Distance (transform.position, desPos) > 0.1f)) {
			//navMeshAgent.SetDestination (desPos);

			//moveDirection = navMeshAgent.velocity;
		} else {
			//navMeshAgent.Stop();

			useNavAgent = false;

			characterController.Move (moveDirection);
			
			if (lookDirection != Vector3.zero) {
				transform.rotation = Quaternion.LookRotation (lookDirection);
			}
		}

		UpdateAnim();

		CalcHealth();
	}

    public void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("InanimateBody")) {
			TransferBody(other.gameObject);
		}
    }

	private void TransferBody(GameObject other) {
		lockInputTime = Time.time + changeBodyInterval;
		changeBodyTime = lockInputTime;

		lookDirection = other.transform.forward;
		moveDirection = Vector3.zero;

		GameObject ragdollInstance = GameObject.Instantiate(ragdollPrefab, transform.position, transform.rotation);
		
		Vector3 prevRootPos = modelRoot.position;
		Quaternion prevRootRot = modelRoot.rotation;

		Utils.CopyTransformsRecurse(other.transform, modelRoot);

		modelRoot.rotation = prevRootRot;
		modelRoot.position = prevRootPos;

		BodyParts ragdollBodyParts = ragdollInstance.GetComponent<BodyParts>();

		theFlame.position = ragdollBodyParts.headBone.transform.position;

		Destroy(other.gameObject);

		health = maxHealth;
	}
	
	public override void Hit(GameObject origin, Vector3 position, float strength) {
		if (invincibleTime < Time.time) {
			return;
		}

		invincibleTime = Time.time + invincibleInterval;

		hurtTime = Time.time + 0.5f;
	}

	private void UpdateInput() {
		float inputX = Input.GetAxis("Horizontal");
		float inputY = Input.GetAxis("Vertical");
		float lookX = Input.GetAxis("Mouse X");
		//float lookY = Input.GetAxis("Mouse Y");

		run = Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);

		bool attackFireball = Input.GetMouseButton (0);
		bool attackBurst = Input.GetMouseButton (1);

		if((Mathf.Abs(inputX) > 0.01) || (Mathf.Abs(inputY) > 0.01))
			useNavAgent = false;

		Vector3 forward = transform.forward;
		Vector3 right = transform.right;

		moveDirection = (forward * inputY + right * inputX) * CalcSpeed() ;

		//lookDirection = Quaternion.Euler(lookY, lookX, 0) * lookDirection;
		lookDirection = Quaternion.Euler(0, lookX * lookSpeed, 0) * lookDirection;

		if ((attackFireballTime > Time.time) || (attackBurstTime > Time.time)) {
		} else if (cooldownTime > Time.time) {
		} else if (attackFireball) {
			attackFireballTime = Time.time + attackInterval;
			cooldownTime = attackFireballTime + cooldownInterval;

			health -= fireballHealthLoss;

		} else if (attackBurst) {
			attackBurstTime = Time.time + attackInterval;
			cooldownTime = attackBurstTime + cooldownInterval;

			health -= burstHealthLoss;
		}
	}

	private float CalcSpeed() {
		float speed = (run? runSpeed: moveSpeed);

		for(int i = grabbers.Count -1; i >= 0; i--) {
			Transform grabber = grabbers[i];

			if(grabber == null) {
				grabbers.RemoveAt(i);
				continue;
			}
		}

		speed -= speed * Mathf.Clamp(grabbers.Count, 0f, 3f) / 3f;

		return speed;
	}

	private void CalcHealth() {
		float factor = 1f;

		if(run) {
			factor = 2f;
		}

		health = health - healthLossFactor * factor * Time.deltaTime;

		body.armR.SetActive(health > 13f);
		body.legL.SetActive(health > 11f);
		body.hips.SetActive(health > 9f);
		body.armL.SetActive(health > 7f);
		body.thorax.SetActive(health > 5f);
		body.legR.SetActive(health > 3f);
		body.head.SetActive(health > 1f);

		if(health <= 0f) {
			health = 0f;

			Death();
		}
	}

	private void Death() {
		GameObject ragdollInstance = GameObject.Instantiate(ragdollPrefab, transform.position, transform.rotation);

		Utils.CopyTransformsRecurse(modelRoot, ragdollInstance.transform);

		BodyParts ragdollBodyParts = ragdollInstance.GetComponent<BodyParts>();
		
		theFlame.transform.SetParent(null);

		TrackObject theFlameTrack = theFlame.GetComponent<TrackObject>();

		theFlameTrack.target = ragdollBodyParts.headBone.transform;

		dead = true;

		Destroy(gameObject);
	}

	public void AddGrabEnemy(Transform enemy) {
		grabbers.Add(enemy);
	}

	public bool IsDead() {
		return dead;
	}

	private void UpdateAnim() {
		anim.SetBool ("running", run);
		anim.SetFloat ("moveH", moveDirection.magnitude);
		anim.SetFloat ("moveV", moveDirection.magnitude);
		anim.SetBool ("fireball", attackFireballTime > Time.time);
		anim.SetBool ("burst", attackBurstTime > Time.time);
		anim.SetFloat ("rise", Mathf.Clamp(changeBodyTime - Time.time, -1f, 1f));
	}
}