using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnInanimates : MonoBehaviour {
	public float evaluationTime = 1f;
	public int minSkeletons = 4;
	public GameObject skeletonPrefab;
	public GameObject[] skeletons;

	Vector3[] skeletonSpawnPoints;
	Quaternion[] skeletonSpawnRotations;
	List<int> killOrder = new List<int>();

	void Awake () {
		skeletonSpawnPoints = new Vector3[skeletons.Length];
		skeletonSpawnRotations = new Quaternion[skeletons.Length];
		for (int i = 0; i < skeletons.Length; i++) {
			GameObject skeleton = skeletons[i];
			skeletonSpawnPoints[i] = new Vector3(skeleton.transform.position.x, skeleton.transform.position.y, skeleton.transform.position.z);
			skeletonSpawnRotations[i] = new Quaternion(skeleton.transform.rotation.x, skeleton.transform.rotation.y, skeleton.transform.rotation.z, skeleton.transform.rotation.w);
		}

		if (skeletons.Length < minSkeletons)
			minSkeletons = skeletons.Length;
	}

	private void OnEnable () {
		InvokeRepeating("evaluateSkeletons", evaluationTime, evaluationTime);
	}

	private void OnDisable () {
		CancelInvoke("evaluateSkeletons");
	}

	void evaluateSkeletons() {
		for (int i = 0; i < skeletons.Length; i++) {
			if (skeletons[i] == null) {
				if (!killOrder.Contains(i)) {
					Debug.Log("Skeleton is used, adding to list.");
					killOrder.Add(i);
					// new skeleton added, check if maximum reached.
					if ((skeletonSpawnPoints.Length - killOrder.Count) < minSkeletons) {
						Debug.Log("Respawning skeleton: " + killOrder[0]);
						GameObject skeleton = Instantiate(skeletonPrefab, skeletonSpawnPoints[killOrder[0]], skeletonSpawnRotations[killOrder[0]]);
						skeletons[killOrder[0]] = skeleton;
						killOrder.RemoveAt(0);
					}
				}
			}
		}
	}
}
