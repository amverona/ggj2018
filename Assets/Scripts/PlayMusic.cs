using UnityEngine;

public class PlayMusic:MonoBehaviour {
	//public Party party;

	public AudioSource[] audioSources;

	[System.Serializable]
	public class MusicData {
		public string name;
		public string artist;
		public AudioClip music;		
	}

	[SerializeField]
	public MusicData[] musics;

	private int idx = -1;

	public bool playOnAwake = true;

	public delegate void MusicChangeEvent(string name, string artist);
	public event MusicChangeEvent OnMusicChanged;

	public void Awake() {
		if(playOnAwake) {
			PlayNext ();
		}
	}

	public void PlayNext() {
		if (musics.Length > 1) {
			idx = Random.Range (0, musics.Length);
		} else {
			idx = 0;
		}

		if(OnMusicChanged != null)
			OnMusicChanged(musics[idx].name, musics[idx].artist);

		foreach(AudioSource audio in audioSources) {
			audio.clip = musics[idx].music;

			audio.Play();
		}

		if(musics[idx] != null)
			Invoke("PlayNext", musics[idx].music.length);
	}
	
	public void Continue() {
		foreach(AudioSource audio in audioSources) {
			audio.Play();
		}

		if(musics[idx] != null)
			Invoke("PlayNext", musics[idx].music.length);
	}
	
	public void Pause() {
		StopAllCoroutines();
		//StopCoroutine ("PlayNext");

		foreach(AudioSource audio in audioSources) {
			audio.Pause();
		}
	}

	public MusicData CurrentMusic() {
		return musics[idx];
	}
}