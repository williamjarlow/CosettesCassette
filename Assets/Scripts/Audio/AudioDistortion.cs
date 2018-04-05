using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioDistortion : MonoBehaviour {

	private AudioMusic audioMusic;
    public Slider distortionSlider;

	void Start (){
		audioMusic = FindObjectOfType<AudioMusic> ();
	}

    private void Update()
    {
        audioMusic.gameMusicEv.setParameterValue("dist_level", distortionSlider.value);
    }

}
