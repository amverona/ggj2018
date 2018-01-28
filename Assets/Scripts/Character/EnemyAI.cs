using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {

    private List<Transform> targets;

    private NavMeshAgent navMeshAgent;

	public Animator anim;

    public GameObject grabPrefab;

	public GameObject ragdollPrefab;

    public Transform modelRoot;

    public void Awake () {
        targets = new List<Transform>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void Update () {
        if(targets.Count > 0) {
            if(targets[0] == null) {
                targets.RemoveAt(0);
                return;                
            }

            Vector3 targetXZ = targets[0].position;

            targetXZ.y = transform.position.y;

            transform.LookAt(targetXZ);

            RaycastHit hit;

            if(Physics.Raycast(targets[0].position, Vector3.down, out hit)) {
                Debug.DrawLine(targets[0].position, hit.point, Color.cyan, 1f, false);

                navMeshAgent.SetDestination(hit.point);
            }
        }

        UpdateAnim();
    }

    public void OnTriggerEnter(Collider other) {
        GameObject otherGO = other.gameObject;

        if(otherGO.layer == LayerMask.NameToLayer("Visibility")) {
			targets.Add(other.transform);
		} else if(otherGO.layer == LayerMask.NameToLayer("Damage")) {
            Death();
        } else if(otherGO.CompareTag("Player")) {
            GrabPlayer(otherGO);
        }
    }

    public void OnTriggerExit(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Visibility")) {
            targets.Remove(other.transform);
		}
    }

	public void OnDrawGizmosSelected()	{
		DebugDrawPath();
	}

	public void DebugDrawPath()	{
		if (navMeshAgent == null || navMeshAgent.path == null)
			return;

		LineRenderer line = this.GetComponent<LineRenderer>();
		
		if (line == null) {
			line = this.gameObject.AddComponent<LineRenderer>();
			line.material = new Material( Shader.Find( "Sprites/Default" ) ) { color = Color.white };
			line.startWidth = 0.1f;
			line.endWidth = 0.1f;
			line.startColor = Color.yellow;
			line.endColor = Color.blue;
		}

        UnityEngine.AI.NavMeshPath path = navMeshAgent.path;

		line.positionCount = path.corners.Length;

		for (int i = 0; i < path.corners.Length; i++) {
			line.SetPosition(i, path.corners[i]);
		}
	}

	private void Death() {
		GameObject ragdollInstance = GameObject.Instantiate(ragdollPrefab, transform.position, transform.rotation);

		Utils.CopyTransformsRecurse(modelRoot, ragdollInstance.transform);

        BodyParts ragdollBodyParts = ragdollInstance.GetComponent<BodyParts>();

        ragdollBodyParts.head.SetActive(false);

		Destroy(gameObject);
	}

	private void GrabPlayer(GameObject playerGO) {
		GameObject grabInstance = GameObject.Instantiate(grabPrefab, transform.position, transform.rotation);

        TrackObject tracker = grabInstance.GetComponent<TrackObject>();

        tracker.target = playerGO.transform;

        PlayerController player = playerGO.GetComponent<PlayerController>();

        player.AddGrabEnemy(grabInstance.transform);

		Destroy(gameObject);
	}

	private void UpdateAnim() {
        //Debug.Log("Velocity: " + navMeshAgent.velocity);

		anim.SetFloat ("moveH", navMeshAgent.velocity.magnitude);
		anim.SetFloat ("moveV", navMeshAgent.velocity.magnitude);
	}
}
