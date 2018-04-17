using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AudioPitch : MonoBehaviour {

	private AudioManager audioManager;
    [SerializeField] private Slider pitchSlider;
    [SerializeField] private PitchDetection pitchDetection;

    const int mercyZone = 5;

	void Start (){
		audioManager = GetComponent<AudioManager> ();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (pitchSlider.value <= ((pitchDetection.transform.position.y - 1) * 10) + mercyZone && pitchSlider.value >= ((pitchDetection.transform.position.y - 1) * 10) - mercyZone)
            audioManager.gameMusicEv.setParameterValue("pitch_sum", 0);
        else
            audioManager.gameMusicEv.setParameterValue("pitch_sum", pitchSlider.value - ((pitchDetection.transform.position.y - 1.5f) * 10));
    }
}
