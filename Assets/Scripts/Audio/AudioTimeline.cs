using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioTimeline : MonoBehaviour {

    [TextArea]
    public string Notes = "Timeline slider needs specific event data: \n Pointer Down: Call function ToggleHold() \n Pointer up: Call function ToggleHold() \n Select: Call function SaveValue() \n Drag: Call function UpdateValues()";

    private AudioManager audioManager;


    [SerializeField]
    private Slider timelineSlider;
    private int temp;

    [Header("Threshold accepted before we slow down the slider")]
    [SerializeField] private float maxSpeedThresholdInMs = 6000;

    [Header("Speed values for slider movement")]
    [SerializeField] private float fastSpeedInMs = 20000;
    [SerializeField] private float slowSpeedInMs = 1500;

    private bool holding = false;
    private float sliderValueAtPush = 0;
    private float valuePushedOn = 0;

    void Start ()
    {
        audioManager = GetComponent<AudioManager>();

        //Find the length of the track and set the max value of the slider to it
        timelineSlider.maxValue = audioManager.GetTrackLength();
    }
	
	
	void Update ()
    {
        if (!holding)
            ChangeOnPlaying();
        if (holding)
            HoldChange();
    }

    public void ChangeTimeline()
    {
        audioManager.gameMusicEv.setTimelinePosition((int)timelineSlider.value);
    }

    // Manually change the slider with touch
    public void HoldChange()
    {
        if (sliderValueAtPush < (valuePushedOn - maxSpeedThresholdInMs) && sliderValueAtPush != valuePushedOn)
            sliderValueAtPush += fastSpeedInMs * Time.deltaTime;

        else if (sliderValueAtPush < valuePushedOn)
            sliderValueAtPush += slowSpeedInMs * Time.deltaTime;


        if (sliderValueAtPush > (valuePushedOn + maxSpeedThresholdInMs))
            sliderValueAtPush -= fastSpeedInMs * Time.deltaTime;

        else if (sliderValueAtPush > valuePushedOn)
            sliderValueAtPush -= slowSpeedInMs * Time.deltaTime;


        timelineSlider.value = sliderValueAtPush;
        audioManager.gameMusicEv.setTimelinePosition((int)sliderValueAtPush);
    }

    // Toggle hold.... 'Cause buttons
    public void ToggleHold()
    {
        if (!holding)
            valuePushedOn = timelineSlider.value;
        holding = !holding;
    }

    // Sliders are stupid and the value is changed BEFORE running code on click events. So current value of slider needs to be saved in order to update slider correctly from original position.
    public void SaveValue()
    {
        sliderValueAtPush = timelineSlider.value;
    }

    // Drag event has to re-save the value to get this correct
    public void UpdateValues()
    {
        valuePushedOn = timelineSlider.value;
    }


    // Update slider according to music when not touching the slider
    void ChangeOnPlaying()
    {
        audioManager.gameMusicEv.getTimelinePosition(out temp);
        timelineSlider.value = temp;
    }
}
