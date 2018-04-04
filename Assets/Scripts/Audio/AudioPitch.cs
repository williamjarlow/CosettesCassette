using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPitch : MonoBehaviour {

	private AudioMusic audioMusic;
    private BasicTouch basicTouch;

	void Start (){
		audioMusic = FindObjectOfType<AudioMusic> ();
        basicTouch = FindObjectOfType<BasicTouch>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            audioMusic.gameMusicEv.setParameterValue("pitch_sum", basicTouch.touchDiff);
        }
        
    }
}
