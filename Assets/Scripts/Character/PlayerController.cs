using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Damageable {

	public CharacterController characterController;

	public Vector3 moveDirection;
	public Vector3 lookDirection;
	public float moveSpeed = 0.1f;
	public float smoothMoveSpeed = 0.1f;
	public float runSpeed = 0.1f;
	public float rotSpeed = 0.1f;
	public float smoothRotSpeed = 0.1f;
	public Camera mainCamera;

	public Animator anim;

	private float hurtTime = -1f;

	private bool run;
	private bool attack;
	private float randomIdx = 0f;
		
	public float invincibleInterval = 0.5f;
	private float invincibleTime = -1f;

	public float attackInterval = 1f;
	private float attackTime = -1f;

	public float cooldownInterval = 0.5f;
	private float cooldownTime = -1f;

	public AttackCollider[] attackColliders;

	private Vector3 desPos;
	private bool useNavAgent;
	
	//private UnityEngine.AI.NavMeshAgent navMeshAgent;

	public bool lockInput;


	void Start() {
		if (mainCamera == null) {
			mainCamera = Camera.main;
		}

		lookDirection = transform.forward;
	
		//navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	void Update () {
		if(lockInput) {
			useNavAgent = false;
			//navMeshAgent.Stop();

			return;
		}

		if (hurtTime > Time.time) {
			run = false;
			attack = false;
			moveDirection = Vector3.zero;
			attackTime = -1f;
		} else {
			UpdateInput ();
		}

		foreach (AttackCollider attackCollider in attackColliders) {
			attackCollider.gameObject.SetActive(attackTime > Time.time);
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

		run = Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);
		attack = Input.GetKey (KeyCode.Space) || Input.GetMouseButton (1);

		if((Mathf.Abs(inputX) > 0.01) || (Mathf.Abs(inputY) > 0.01))
			useNavAgent = false;

		Vector3 forward = transform.forward;
		Vector3 right = transform.right;

		moveDirection = (forward * inputY + right * inputX) * (run? runSpeed: moveSpeed);

		lookDirection = Quaternion.Euler(0, lookX, 0) * lookDirection;

		if (attackTime > Time.time) {
		} else if (cooldownTime > Time.time) {
		} else if (attack) {
			attackTime = Time.time + attackInterval;
			cooldownTime = attackTime + cooldownInterval;

			randomIdx = UnityEngine.Random.value;
		}
	}

	private void UpdateAnim() {
		anim.SetFloat ("randomIdx", randomIdx);
		anim.SetBool ("running", run);
		anim.SetFloat ("moveH", moveDirection.magnitude);
		anim.SetFloat ("moveV", moveDirection.magnitude);
		anim.SetBool ("hurting", hurtTime > Time.time);
		anim.SetBool ("attacking", attackTime > Time.time);
	}
}


