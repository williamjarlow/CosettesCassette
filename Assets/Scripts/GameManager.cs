using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum instruments { drums, synth, vocals, guitar, bass, pitch };
public class GameManager : MonoBehaviour
{

    [HideInInspector] public AudioDistortion audioDistortion;
    [HideInInspector] public OverallCorruption overallCorruption;
    [HideInInspector] public AudioPitch audioPitch;
    public AudioManager audioManager;
    public GameObject corruptionHandler;
    public GameObject timelineSlider;
    public GameObject uiHandler;
    public GameObject uiParent;
    public GameObject pitchSlider;
    public ButtonScript minigameButton;
    public StageClearVFX stageClearVFX;

    const int timelineOffset = 100;

    [HideInInspector] public bool LevelCleared;
    [HideInInspector] public bool LevelPerfected;
    [HideInInspector] public bool recording;

    private ButtonDisabler buttonDisabler;

    [HideInInspector] public float pitch;
    [HideInInspector] public int lengthOfSong;
    [HideInInspector] public int currentSegmentIndex;
    [HideInInspector] public int timeStamp;
    [Header("How far into the corrupted area we are allowed without snapping to earlier segment when using skip")]
    [SerializeField]
    private int allowedProgressIntoBarInMs = 5000;

    // Tolerance to make sure we run certain functions for entering a segment
    private const int tolerance = 150;

    public instruments selectedInstrument = instruments.drums;

    [Header("Stickers for this level")]
    public Sprite stickerForGood;
    public Sprite stickerForPerfect;


    void Awake()
    {
        audioDistortion = audioManager.GetComponent<AudioDistortion>();
        audioPitch = audioManager.GetComponent<AudioPitch>();
        overallCorruption = corruptionHandler.GetComponent<OverallCorruption>();
        buttonDisabler = uiHandler.GetComponent<ButtonDisabler>();
    }

    private void Start()
    {
        Debug.Assert(this.gameObject.tag == "GameManager", "Set GameManager tag to GameManager");
    }

    private void Update()
    {
        timeStamp = (int)audioManager.GetTimeLinePosition();
    }

    public instruments SelectedInstrument
    {
        get { return selectedInstrument; }
        set { selectedInstrument = value; }
    }


    // ** General functions for different mechanics ** //


    public void ToggleRecord()
    {
        // If the current track is playing
        if (audioManager.GetTimeLinePosition() > 0)
        {
            
            // If we are not recording --> start recording
            if (recording == false)
            {
                // Find and jump to the closest segment
                SnapToClosestSegment();
                // Start recording
                recording = true;
                audioManager.PlayRecordStart();
            }

            // If we are recording --> stop recording
            else if (recording)
            {
                recording = false;
                audioManager.PlayRecordStop();
            }
        }
    }

    public void Listen()
    {
        recording = false;

		audioManager.PlayRecordStop();
    }

    public void FindClosestSegment()
    {
        // Declare an int array with the same size as the number of corrupted segments
        int[] timelineDifference = new int[overallCorruption.durations.Count];

        for (int i = 0; i < overallCorruption.durations.Count; i++)
        {
            // Find the distance between start or endpoint of the segment
            if (timeStamp > 0 && timeStamp < overallCorruption.durations[i].start)
            {
                timelineDifference[i] = Mathf.Abs(timeStamp - overallCorruption.durations[i].start);
            }

            else if (timeStamp > 0 && timeStamp > overallCorruption.durations[i].stop)
            {
                timelineDifference[i] = Mathf.Abs(timeStamp - overallCorruption.durations[i].stop);
            }
        }

        int closestSegment = timelineDifference[0];
        int closestSegmentIndex = 0;

        // Find the closest segment by looping through the array and comparing the elements
        for (int i = 0; i < timelineDifference.Length; i++)
        {
            if (closestSegment > timelineDifference[i])
            {
                closestSegment = timelineDifference[i];
                closestSegmentIndex = i;
            }

        }

        // Set the current segment index to the closest one
        currentSegmentIndex = closestSegmentIndex;

		audioManager.PlaySnapSound ();
    }


    // If 'YES' is pressed during confirmation --> Delete previous recordings, start recording and disable the confirmation window
    public void YesConfirmation(GameObject confirmationObj)
    {
        // If we are at the music cassette 
        if (!audioManager.switchedToAudioLog)
        {
            confirmationObj.SetActive(false);
            buttonDisabler.DisableButtons();

            // Snap to the current segment start, delete previous recordings, start recording
            audioManager.gameMusicEv.setTimelinePosition(overallCorruption.durations[currentSegmentIndex].start - timelineOffset); //Slight offset to make sure that corruption gets entered properly
            overallCorruption.durations[currentSegmentIndex].ClearRecordings();
            recording = true;
        }

    }

    // If 'NO' is pressed during confirmation --> disable the confirmation window
    public void NoConfirmation(GameObject confirmationObj)
    {
        confirmationObj.SetActive(false);
        buttonDisabler.EnableButtons();
    }

    public void SnapToClosestSegment()
    {
        FindClosestSegment();

        // Tolerance to make sure we run certain functions for entering a segment
        audioManager.gameMusicEv.setTimelinePosition(overallCorruption.durations[currentSegmentIndex].start - tolerance);
    }

    public void SnapToClosestSegmentInFront()
    {
        if (!recording)
        {
            FindClosestSegment();
            if (currentSegmentIndex == overallCorruption.durations.Count - 1)       // If last segment is closest
            {
                audioManager.gameMusicEv.setTimelinePosition(lengthOfSong);         // Snap to end of song
                return;
            }
            if (currentSegmentIndex == 0 && overallCorruption.durations[currentSegmentIndex].start > audioManager.GetTimeLinePosition())
                // If first segment is closest and timeline is before that segment
                audioManager.gameMusicEv.setTimelinePosition(overallCorruption.durations[currentSegmentIndex].start);                    // Snap to start of first segment
            else
                audioManager.gameMusicEv.setTimelinePosition(overallCorruption.durations[currentSegmentIndex + 1].start);                // In all other cases, snap to next segment    
        }
    }

    public void SnapToClosestSegmentBehind()
    {
        if (!recording)
        {
            FindClosestSegment();
            if (currentSegmentIndex == 0 && overallCorruption.durations[currentSegmentIndex].start + allowedProgressIntoBarInMs > audioManager.GetTimeLinePosition()) // If first segment is closest and we are less than "allowedProgressIntoBarInMs" into the segment
            {
                audioManager.gameMusicEv.setTimelinePosition(0);                                                                                                      // Snap to beginning of song
                return;
            }
            else if (currentSegmentIndex == 0)                                                                                                                        // Else, if first segment
            {
                audioManager.gameMusicEv.setTimelinePosition(overallCorruption.durations[currentSegmentIndex].start);                                                 // Snap to start of first segment
                return;
            }
            // If last segment is closest and we are less than "allowedProgressIntoBarInMs" into the segment
            if (currentSegmentIndex == overallCorruption.durations.Count - 1 && overallCorruption.durations[currentSegmentIndex].start + allowedProgressIntoBarInMs < audioManager.GetTimeLinePosition())
                audioManager.gameMusicEv.setTimelinePosition(overallCorruption.durations[currentSegmentIndex].start);                                                 // Snap to start of last segment
            if (currentSegmentIndex != 0 && overallCorruption.durations[currentSegmentIndex].start + allowedProgressIntoBarInMs < audioManager.GetTimeLinePosition()) // If any other segment except first or last and we are less than "allowedProgressIntoBarInMs" into the segment
                audioManager.gameMusicEv.setTimelinePosition(overallCorruption.durations[currentSegmentIndex].start);                                                 // Snap to start of that segment
            else
                audioManager.gameMusicEv.setTimelinePosition(overallCorruption.durations[currentSegmentIndex - 1].start);                                             // Else, snap to start of earlier segment
        }
    }


    // ** OLD Recording button ** //
    // A confirmation is no longer needed and thus this function should not be needed
    public void LegacyRecord(GameObject confirmationObj)
    {
        FindClosestSegment();

        // Loop through the lists of recordings in the current segment

        // If there are no previous recordings, start recording
        if (timeStamp > 0 && overallCorruption.durations[currentSegmentIndex].HasRecordings() == false)
        {
            // Snap to the nearest corrupted area
            SnapToClosestSegment();
            Debug.Log("Snapped and started recording");

            recording = true;
            buttonDisabler.DisableButtons();
        }

        // If there are previous recordings, display the prompt window
        else if (timeStamp > 0 && overallCorruption.durations[currentSegmentIndex].HasRecordings() == true)
        {
            confirmationObj.SetActive(true);
        }
    }

    void DisableMechanics()
    {
        corruptionHandler.SetActive(false);
    }

}


/*public void Record()
{
    // If the current track is playing
    if(audioManager.GetTimeLinePosition() > 0)
    {
        // Find and jump to the closest segment
        SnapToClosestSegment();
        // Start recording and disable buttons
        recording = true;
        //buttonDisabler.DisableButtons();

        audioManager.PlayRecordStart();
    }

}*/



/*    public void Record(GameObject confirmationObj)
    {
        FindClosestSegment();

        if (timeStamp > 0 && overallCorruption.durations[currentSegmentIndex].GetDrumRecordings().Count == 0)
            {
                // Snap to the nearest corrupted area
                audioManager.gameMusicEv.setTimelinePosition(overallCorruption.durations[currentSegmentIndex].start);

                recording = true;
                buttonDisabler.DisableButtons();
            }

            // If there is the current timeline position is not 0 and is inside a segment and there IS a previous recording at segment [i]
            //else if (timeStamp > 0 && overallCorruption.durations[currentSegmentIndex].GetDrumRecordings().Count > 0)
            {
                confirmationObj.SetActive(true);
            }
        }
    }
*/
