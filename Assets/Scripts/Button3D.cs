using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button3D : MonoBehaviour {
	public Camera cam;

	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				//Debug.Log("Mouse Down Hit the following object: " + hit.collider.name);
				if (hit.collider.gameObject == this.gameObject)
					Click();
			}
		}
	}

	virtual public void Click() {

	}
}
