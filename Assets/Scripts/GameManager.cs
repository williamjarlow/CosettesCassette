using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum instruments { drums, synth, vocals, guitar, bass, pitch };
public class GameManager : Singleton<GameManager> {

    [HideInInspector] public AudioDistortion audioDistortion;
    [HideInInspector] public OverallCorruption overallCorruption;
    [HideInInspector] public AudioPitch audioPitch;
    public GameObject drumMechanic;
    public AudioManager audioManager;
    public GameObject corruptionHandler;
    public GameObject timelineSlider;
    public GameObject uiHandler;
    public GameObject pitchSlider;

    [HideInInspector] public bool LevelCleared;
    [HideInInspector] public bool recording;

    private ButtonDisabler buttonDisabler;

    [HideInInspector] public float pitch;
    [HideInInspector] public float posInSong;
    [HideInInspector] public float lengthOfSong;
    [HideInInspector] public int currentSegmentIndex;
    [HideInInspector] public int timeStamp;


    public instruments currentInstrument = instruments.drums;

    

	void Awake ()
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

    public instruments CurrentInstrument{
        get { return currentInstrument; }
        set { currentInstrument = value; }
    }

    private void Update()
    {
        audioManager.gameMusicEv.getTimelinePosition(out timeStamp);
    }


    // ** General functions for different mechanics ** //

    public void Record(GameObject confirmationObj)
    {
        FindClosestSegment();

        // Loop through the lists of recordings in the current segment
        
            // If there are no previous recordings, start recording
            if(timeStamp > 0 && overallCorruption.durations[currentSegmentIndex].HasRecordings() == false)
            {
                // Snap to the nearest corrupted area
                audioManager.gameMusicEv.setTimelinePosition(overallCorruption.durations[currentSegmentIndex].start);
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

    public void Listen()
    {
        buttonDisabler.EnableButtons();
        recording = false;
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
            audioManager.gameMusicEv.setTimelinePosition(overallCorruption.durations[currentSegmentIndex].start);
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



}


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