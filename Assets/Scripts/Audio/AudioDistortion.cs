using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDistortion : MonoBehaviour {

	private AudioMusic audioMusic;

	void Start (){
		audioMusic = FindObjectOfType<AudioMusic> ();
	}

	public void distOn (){
		audioMusic.gameMusicEv.setParameterValue("dist_onoff", 1);
	}

	public void distOff (){
		audioMusic.gameMusicEv.setParameterValue("dist_onoff", 0);
	}

}
