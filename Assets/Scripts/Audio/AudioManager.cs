using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	private AudioMusic audioMusic;
	private AudioDistortion audioDistortion;

	void Start (){
		audioMusic = FindObjectOfType<AudioMusic> ();
		audioDistortion = FindObjectOfType<AudioDistortion> ();
	}

	public void AudioPlayMusic (string musicTrack){
        //audioMusic = FindObjectOfType<AudioMusic>();
        audioMusic.playMusic (musicTrack);
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

	public void AudioDistOn (){
		audioDistortion.distOn ();
	}

	public void AudioDistOff (){
		audioDistortion.distOff ();
	}
}
