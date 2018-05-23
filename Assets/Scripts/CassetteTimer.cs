using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// ** Written by Hannes Gustafsson ** //

// Script to display the current time in the song
public class CassetteTimer : MonoBehaviour {

    private AudioManager audioManager;
    private Text text;
    private float currentTime;
    private string seconds;
    private string minutes;

	void Start ()
    {
        audioManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>().audioManager;
        text = GetComponent<Text>();
	}
	
	void Update ()
    {
        currentTime = audioManager.GetTimeLinePosition();
        
        // Convert to seconds
        currentTime /= 1000;

        // Conversions
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);

        // If seconds is < 10, display another 0
        if (timeSpan.Seconds < 10)
            seconds = "0" + timeSpan.Seconds.ToString();

        else
            seconds = timeSpan.Seconds.ToString();

        minutes = timeSpan.Minutes.ToString();


        text.text = "0" + minutes + ":" + seconds;
        
    }
}
