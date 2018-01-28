using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {
	public AudioClip[] musicClips;
	AudioSource _player;

	private void Awake () {
		_player = GetComponent<AudioSource>();
	}

	void Start () {
		ChooseMusic();
	}

	void ChooseMusic () {
		AudioClip clip = musicClips[Random.Range(0, musicClips.Length)];
		float timeToWait = clip.length * Random.Range(2,4);
		_player.clip = clip;
		_player.Play();
		Invoke("ChooseMusic", timeToWait);
	} 
	
	void FixedUpdate () {
		//follow camera
		this.transform.position = Camera.main.transform.position;
	}
}
