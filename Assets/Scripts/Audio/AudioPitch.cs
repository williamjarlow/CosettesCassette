using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPitch : MonoBehaviour {

	private AudioMusic audioMusic;

	void start (){
		audioMusic = FindObjectOfType<AudioMusic> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
