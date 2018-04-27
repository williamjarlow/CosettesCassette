using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMusic : MonoBehaviour {

	

    private AudioManager audioManager;


    private void Start()
    {
        audioManager = GameManager.Instance.audioManager;
    }
}
