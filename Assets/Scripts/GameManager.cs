using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class GameManager : MonoBehaviour {

	void Awake () {
/*#if UNITY_EDITOR
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;
//# else*/
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
//#endif
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
			Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
		}
	}
}
