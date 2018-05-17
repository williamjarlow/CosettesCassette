using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMusic : MonoBehaviour {

    GameManager gameManager;

    private AudioManager audioManager;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        audioManager = gameManager.audioManager;
    }
}
