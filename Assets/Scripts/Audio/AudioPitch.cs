using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AudioPitch : MonoBehaviour {

	private AudioMusic audioMusic;
    public Slider pitchSlider;

	void Start (){
		audioMusic = FindObjectOfType<AudioMusic> ();
	}
	
	// Update is called once per frame
	void Update ()
    {
        {
            audioMusic.gameMusicEv.setParameterValue("pitch_sum", pitchSlider.value);
        }
        
    }
}
