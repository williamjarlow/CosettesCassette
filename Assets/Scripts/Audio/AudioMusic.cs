using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMusic : MonoBehaviour {

    GameManager gameManager;

    private AudioManager audioManager;

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        audioManager = gameManager.audioManager;
    }
}
