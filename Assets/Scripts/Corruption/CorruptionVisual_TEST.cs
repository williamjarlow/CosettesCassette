using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorruptionVisual_TEST : MonoBehaviour {

    private RectTransform corruptedArea;

    private AudioManager audioManager;

    private int pixelTrackLength;

	void Start ()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        //pixelTrackLength = (int)audioManager.GetTrackLength() / 
            //32000 / 1000 = 32
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
