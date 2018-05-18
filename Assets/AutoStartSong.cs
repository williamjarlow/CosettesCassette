using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoStartSong : MonoBehaviour {

    [SerializeField] private AudioManager audioManager;
	// Use this for initialization
	void Start () {
        audioManager.AudioPlayMusic();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
