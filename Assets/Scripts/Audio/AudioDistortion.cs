using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioDistortion : MonoBehaviour {

	private AudioMusic audioMusic;
    [SerializeField] private Slider distortionSlider;

	void Start (){
		audioMusic = GetComponent<AudioMusic> ();
	}

    private void Update()
    {
        
    }

    public void ChangeDistortion()
    {
        audioMusic.gameMusicEv.setParameterValue("dist_level", distortionSlider.value);
    }

}
