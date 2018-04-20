using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AudioPitch : MonoBehaviour {

	private AudioManager audioManager;
    [SerializeField] private Slider pitchSlider;

    const int mercyZone = 5;

	void Start (){
		audioManager = GetComponent<AudioManager> ();
	}
	
	// Update is called once per frame
	void Update ()
    {
    }

    public void SetPitch(float newPitch)
    {
        audioManager.gameMusicEv.setParameterValue("pitch_sum", newPitch);
    }

}
