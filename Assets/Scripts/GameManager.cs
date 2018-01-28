using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class GameManager : MonoBehaviour {

	public GameObject gameOver;
	public PlayerController player;

	void Awake () {
/*#if UNITY_EDITOR
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined; 
//# else*/
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
//#endif

		gameOver.SetActive(false);

		if(player == null) player = Transform.FindObjectOfType<PlayerController>();
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
			Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
		}

		if(player.IsDead() && !gameOver.activeSelf) {
			gameOver.SetActive(true);
		}
	}
}
