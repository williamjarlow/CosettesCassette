using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoStartSong : MonoBehaviour {

    [SerializeField] private AudioManager audioManager;

	void Start () {
        audioManager.AudioPlayMenuMusic();
	}
}
