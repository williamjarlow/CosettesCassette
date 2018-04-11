using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioDistortion : MonoBehaviour {

	private AudioManager audioManager;
    [SerializeField] private Slider distortionSlider;

	void Start (){
		audioManager = GetComponent<AudioManager> ();
	}
      
    private void Update()
    {
    }

    public void ChangeDistortion()
    {
        audioManager.gameMusicEv.setParameterValue("dist_level", distortionSlider.value);
    }

}
