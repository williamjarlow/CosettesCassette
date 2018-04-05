using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioTimeline : MonoBehaviour {

    private AudioMusic audioMusic;
    private FMOD.Studio.EventDescription eventDesc;

    public Slider timelineSlider;

    private int temp;

    void Start ()
    {
        audioMusic = GetComponent<AudioMusic>();

        //Find the length of the track and set the max value of the slider to it
        eventDesc = FMODUnity.RuntimeManager.GetEventDescription(audioMusic.musicPath);
        eventDesc.getLength(out temp);
        timelineSlider.maxValue = temp;

        //Adds a listener to the main slider and invokes a method when the value changes.
        //timelineSlider.onValueChanged.AddListener(delegate { ChangeTimeline(); });
    }
	
	
	void Update ()
    {
        //Moves the slider according to which part of the track is being played
         audioMusic.gameMusicEv.getTimelinePosition(out temp);
         timelineSlider.value = temp;

    }

    public void ChangeTimeline()
    {
        audioMusic.gameMusicEv.setTimelinePosition((int)timelineSlider.value);
    }
}
