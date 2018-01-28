using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour {
	public Vector3 areaSize;
	public GameObject prefab;
	
	[Range(10,1000)]
	public int startingAmount;
	[Range(10,500)]
	public int minAmount;

	List<GameObject> enemies;
	float sqrAmount;
	public int currentAmount;

	float checkDelay = 3.0f;

	private void Awake () {
		enemies = new List<GameObject>();
		sqrAmount = Mathf.Sqrt(startingAmount);
	}

	private void Start () {
		while (enemies.Count < startingAmount) {
			CreateEnemy();
		}
		currentAmount = enemies.Count;
	}

	private void OnEnable () {
		InvokeRepeating("CheckEnemyCount", checkDelay, checkDelay);
	}

	private void OnDisable () {
		CancelInvoke();
	}

	void CheckEnemyCount() {
		List<int> toRemove = new List<int>();
		for (int i = 0; i < enemies.Count; i++) {
			if (enemies[i] == null)
				toRemove.Add(i);
		}

		for (int j = toRemove.Count; j >= 0; j--) {
			enemies.RemoveAt(j);
		}

		while (enemies.Count < minAmount)
			CreateEnemy();
		currentAmount = enemies.Count;
	}

	void CreateEnemy() {
		Vector3 pos = this.transform.position + new Vector3(Random.value * areaSize.x - areaSize.x/2f, 0f, Random.value * areaSize.z - areaSize.z / 2f);

		//Ray ray = new Ray(pos, Vector3.down);
		//RaycastHit[] hit = Physics.RaycastAll(ray, 100f);
		//if ((hit != null) && (hit.Length > 0)) {
		//	Debug.Log("Raycast working.");
		//	GameObject enemy = Instantiate(prefab, hit[0].point + Vector3.up * 0.5f, Quaternion.identity, this.transform);
		//	enemies.Add(enemy);
		//	return;
		//}
		NavMeshHit myNavHit;
		if (NavMesh.SamplePosition(pos, out myNavHit, 20f, NavMesh.AllAreas)) {			
			GameObject enemy = Instantiate(prefab, myNavHit.position, Quaternion.identity, this.transform);
			enemies.Add(enemy);
		} else {
			Debug.LogWarning("Nothing to hit on this position: " + pos);
		}
	}

	private void OnDrawGizmosSelected () {
		Gizmos.DrawWireCube(this.transform.position, areaSize);
	}
}
