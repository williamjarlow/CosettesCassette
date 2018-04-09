using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AudioPitch : MonoBehaviour {

	private AudioMusic audioMusic;
    [SerializeField] private Slider pitchSlider;

	void Start (){
		audioMusic = GetComponent<AudioMusic> ();
	}
	
	// Update is called once per frame
	void Update ()
    {
        {
            audioMusic.gameMusicEv.setParameterValue("pitch_sum", pitchSlider.value);
        }
        
    }
}
