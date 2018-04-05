using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	private AudioMusic audioMusic;
	private AudioDistortion audioDistortion;

	void Start (){
		audioMusic = GetComponent<AudioMusic> ();
		audioDistortion = GetComponent<AudioDistortion> ();
	}

	public void AudioPlayMusic (string musicTrack){
        //audioMusic = FindObjectOfType<AudioMusic>();
        audioMusic.playMusic ();
	}

	public void AudioStopMusic (){
		audioMusic.stopMusic ();
	}

	public void AudioPauseMusic (){
		audioMusic.pauseMusic ();
	}

	public void AudioUnpauseMusic (){
		audioMusic.unpauseMusic ();
	}

}
