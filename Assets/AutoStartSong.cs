using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoStartSong : MonoBehaviour {

    public GameManager gameManager;
	// Use this for initialization
	void Start () {
        gameManager.audioManager.AudioPlayMusic();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
