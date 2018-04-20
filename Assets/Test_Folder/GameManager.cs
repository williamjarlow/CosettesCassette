using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : Singleton<GameManager> {

    [HideInInspector] public AudioDistortion audioDistortion;
    [HideInInspector] public OverallCorruption overallCorruption;
    [HideInInspector] public AudioPitch audioPitch;
    public GameObject drumMechanic;
    public AudioManager audioManager;
    public GameObject corruptionHandler;
    public GameObject timelineSlider;

    [HideInInspector] public float pitch;
    [HideInInspector] public float posInSong;
    [HideInInspector] public float lengthOfSong;

    

	void Awake ()
    {
        audioDistortion = audioManager.GetComponent<AudioDistortion>();
        audioPitch = audioManager.GetComponent<AudioPitch>();
        overallCorruption = corruptionHandler.GetComponent<OverallCorruption>();
    }

    private void Start()
    {
        Debug.Assert(this.gameObject.tag == "GameManager", "Set GameManager tag to GameManager");
    }


    void Update ()
    {

	}

    
}
