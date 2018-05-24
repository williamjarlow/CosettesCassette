using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAndCloseGameHandler : MonoBehaviour {

    [SerializeField] private AudioManager audioManager;
    private AudioManager audioManHolder;
    private GameManager gameManager;

	void Start () {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        audioManHolder = audioManager;
    }

    private void Update()
    {
        if(GameObject.FindWithTag("GameManager").GetComponent<GameManager>() != null)
        {
            gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            audioManager = gameManager.audioManager;
        }
        else
        {
            audioManager = audioManHolder;
        }

    }

    void OnApplicationQuit()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        audioManager.musicChanGroup.setPan(0);
        audioManager.musicChanSubGroup.setPan(0);
    }
}
