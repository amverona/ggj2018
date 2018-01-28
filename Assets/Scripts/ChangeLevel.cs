using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : Button3D {
	public string levelName;

	override public void Click() {
		changeLevel();
	}

	public void changeLevel() {
		SceneManager.LoadScene(levelName);
	}
}
