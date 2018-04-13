using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorruptionVisual_TEST : MonoBehaviour {

    private RectTransform corruptedArea;

    private AudioManager audioManager;

	void Start ()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
