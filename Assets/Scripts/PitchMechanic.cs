using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PitchMechanic : MonoBehaviour
{

    private AudioManager audioManager;
    private OverallCorruption overallCorruption;
    private Slider pitchSlider;

    private int timeStamp;

    [Tooltip("Time tolerance in ms when comparing timeline position and recorded beats")] [SerializeField] private int tolerance;

    void Start()
    {
        audioManager = GameManager.Instance.audioManager;
        overallCorruption = GameManager.Instance.overallCorruption;
        pitchSlider = GameManager.Instance.pitchSlider.GetComponent<Slider>();

    }

    // Update is called once per frame
    void Update()
    {
        // Not necessary, but this way you dont have to write GameManager.Instance.timeStamp every time
        timeStamp = GameManager.Instance.timeStamp;

        // If the track has started, we are in a 'PITCH' segment and we are recording -->
        if (timeStamp > 0 && overallCorruption.durations[GameManager.Instance.currentSegmentIndex].recordingType == Duration.RecordingType.PITCH
            && GameManager.Instance.recording == true)
        {
            // Record time- and pitch slider values
            overallCorruption.durations[GameManager.Instance.currentSegmentIndex].AddPitchRecordings(timeStamp, pitchSlider.value);

        }

        // Recreate the recorded pitch slider values
        else if (GameManager.Instance.recording == false && !audioManager.switchedToAudioLog)
        {
            // Loop through the corrupted segments
            for (int i = 0; i < overallCorruption.durations.Count; i++)
            {
                // Check if we are in a corrupted segment
                if (timeStamp >= overallCorruption.durations[i].start && timeStamp < overallCorruption.durations[i].stop)
                {
                    // Loop through the list of recorded pitches at segment [i]
                    for (int j = 0; j < overallCorruption.durations[i].GetPitchRecordings().Count; j++)
                    {
                        // Match the current time stamp with the recorded time stamp
                        if (timeStamp <= overallCorruption.durations[i].GetPitchRecordings()[j].Key + tolerance / 2 && timeStamp >= overallCorruption.durations[i].GetPitchRecordings()[j].Key - tolerance / 2)
                        {
                            pitchSlider.value = overallCorruption.durations[i].GetPitchRecordings()[j].Value;
                        }
                    }
                }
            }

        }

    }
}
