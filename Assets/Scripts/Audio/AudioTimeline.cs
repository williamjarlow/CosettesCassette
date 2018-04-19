using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioTimeline : MonoBehaviour
{

    [TextArea]
    public string Notes = "Timeline slider needs specific event data to function properly: \n Pointer Down: Call function ToggleHold(); \n Pointer up: Call function ToggleHold(); \n Select: Call function SaveValue(); \n Drag: Call function UpdateValues();";

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

    [TextArea]
    public string MoreNotes = "Mask is under 'Hidden Timeline Images'\n Mask should start inactive as we only want it to show when touching the timeline bar.\n The mask itself needs a MaskingMaterial and the images to hide needs a HideMaterial.";

    public GameObject timelineMask;
    public GameObject timelineBar;

    private float maskStartPos = 0;

    private float songToImageLengthConversion = 0;

    void Start()
    {

        audioManager = GetComponent<AudioManager>();
        //Find the length of the track and set the max value of the slider to it
        timelineSlider.maxValue = audioManager.GetTrackLength();

        // Get start position of timeline bar in x
        maskStartPos = timelineBar.transform.position.x - (timelineBar.GetComponent<RectTransform>().sizeDelta.x / 2);

        // Convert length of song to size of image to compare where in timeline we're at
        songToImageLengthConversion = timelineSlider.maxValue / timelineBar.GetComponent<RectTransform>().sizeDelta.x;
    }


    void Update()
    {
        if (!holding)
        {
            ChangeOnPlaying();
        }

        if (holding)
        {
            HoldChange();
        }
    }

    public void ChangeTimeline()
    {
        audioManager.gameMusicEv.setTimelinePosition((int)timelineSlider.value);
    }

    // Manually change the slider with touch
    public void HoldChange()
    {
        // Stop player from breaking song by forcing it to play the exact same moment over and over again.
        if (Mathf.Abs(sliderValueAtPush - valuePushedOn) < 100) // Yes we have a magic number here!
            return;

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

        // Update mask according to timeline bar
        timelineMask.transform.localPosition = new Vector3(maskStartPos + (timelineSlider.value / songToImageLengthConversion), timelineMask.transform.localPosition.y, 0);
    }

    // Toggle hold.... 'Cause buttons
    public void ToggleHold()
    {
        if (!holding)
        {
            valuePushedOn = timelineSlider.value;
            timelineMask.SetActive(true);
            holding = true;
            return;
        }

        if (holding)
        {
            timelineMask.SetActive(false);
            holding = false;
            return;
        }
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
