using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioTimeline : MonoBehaviour
{

    [TextArea]
    public string Notes = "Timeline slider needs specific event data to function properly: \n Pointer Down: Call function ToggleHold(); \n Pointer up: Call function ToggleHold(); \n Select: Call function SaveValue(); \n Drag: Call function UpdateValues();";

    private AudioManager audioManager;

    private Slider timelineSlider;
    private GameObject timelineBar;
    public GameObject timelineMaskParent;

    private int temp;

    [Header("Threshold accepted before we slow down the slider")]
    [SerializeField]
    private float maxSpeedThresholdInMs = 6000;

    GameManager gameManager;

    [Header("Speed values for slider movement")]
    [SerializeField]
    private float fastSpeedInMs = 20000;
    [SerializeField]
    private float slowSpeedInMs = 1500;

    [Header("Speed values for cassette animation, if used")]
    public float cassetteNormalSpeed = 50;
    public float cassetteSlowSpeed = 200;
    public float cassetteFastSpeed = 500;
    [HideInInspector] public bool movingSlow = false;
    [HideInInspector] public bool movingFast = false;
    [HideInInspector] public bool movingForward = true;

    private bool holding = false;
    private float sliderValueAtPush = 0;
    private float valuePushedOn = 0;

    [TextArea]
    public string MoreNotes = "Mask parent is under 'Hidden Timeline Images'\n Mask should start inactive as we only want it to show when touching the timeline bar.\n The mask itself needs a MaskingMaterial and the images to hide needs a HideMaterial.";

    private float maskStartPos = 0;

    private float songToImageLengthConversion = 0;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
        timelineSlider = gameManager.timelineSlider.GetComponent<Slider>();
        timelineBar = gameManager.timelineSlider;
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
        if (!holding || gameManager.recording)
        {
            ChangeOnPlaying();
        }
        else if (holding)
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
        if (!gameManager.recording)
        {
            // Stop player from breaking song by forcing it to play the exact same moment over and over again.
            if (Mathf.Abs(sliderValueAtPush - valuePushedOn) < 100) // Yes we have a magic number here!
            {
                DecideSpeedAndDirection("moveNormal");
                return;
            }
			if (sliderValueAtPush < (valuePushedOn - maxSpeedThresholdInMs) && sliderValueAtPush != valuePushedOn) {
				DecideSpeedAndDirection ("fastForward");
				audioManager.SetSkipPitch (100f);
			}

			else if (sliderValueAtPush < valuePushedOn) {
				DecideSpeedAndDirection ("slowForward");
				audioManager.SetSkipPitch (0f);
			}

			if (sliderValueAtPush > (valuePushedOn + maxSpeedThresholdInMs)) {
				DecideSpeedAndDirection ("fastBackwards");
				audioManager.SetSkipPitch (100f);
			}

			else if (sliderValueAtPush > valuePushedOn) {
				DecideSpeedAndDirection ("slowBackwards");
				audioManager.SetSkipPitch (0f);
			}


            timelineSlider.value = sliderValueAtPush;
            audioManager.gameMusicEv.setTimelinePosition((int)sliderValueAtPush);

            // Update mask according to timeline bar
            timelineMaskParent.transform.localPosition = new Vector3(maskStartPos + (timelineSlider.value / songToImageLengthConversion), timelineMaskParent.transform.localPosition.y, 0);
        }
    }

    // Toggle hold.... 'Cause buttons
    public void ToggleHold()
    {
        UpdateValues();
        if (!holding)
        {
            if (!gameManager.recording)
                timelineMaskParent.SetActive(true);
            holding = true;
            return;
        }

        if (holding)    // Reset all values, bools are for CassetteAnimation
        {
            timelineMaskParent.SetActive(false);
            holding = false;
            movingSlow = false;
            movingFast = false;
            movingForward = true;
            return;
        }
    }

    private void DecideSpeedAndDirection(string speedAndDirection)
    {
        // Set values depending on where we are touching on the timeline
        switch (speedAndDirection)
        {
            case "fastForward":
                sliderValueAtPush += fastSpeedInMs * Time.deltaTime;
                movingSlow = false;
                movingFast = true;
                movingForward = true;
                break;

            case "slowForward":
                sliderValueAtPush += slowSpeedInMs * Time.deltaTime;
                movingSlow = true;
                movingFast = false;
                movingForward = true;
                break;

            case "fastBackwards":
                sliderValueAtPush -= fastSpeedInMs * Time.deltaTime;
                movingSlow = false;
                movingFast = true;
                movingForward = false;
                break;

            case "slowBackwards":
                sliderValueAtPush -= slowSpeedInMs * Time.deltaTime;
                movingSlow = true;
                movingFast = false;
                movingForward = false;
                break;

            default:
                movingForward = true;
                movingSlow = false;
                movingFast = false;
                break;
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
