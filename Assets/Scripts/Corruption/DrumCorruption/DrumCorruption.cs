using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum Timing
{
    miss,
    okay,
    perfect
};

public class DrumCorruption : CorruptionBaseClass
{
    List<Timing> completedBeats = new List<Timing>();

    DrumMechanic drumMechanic;

    DrumCorruptionHandler drumCorruptionHandler;

    [SerializeField]
    [Range(1, 1)]
    [Header("Value/Penalty per hit/miss")]
    [Tooltip("Hitting a perfect gives a constant value of 1. Changing this will break the corruption calculations: if (beat == Timing.perfect){ corruptionClearedPercent += perfectHitValue / beats.Count;}")]
    float perfectHitValue = 1;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("This value decides how impactful an okay will be. A perfect gives a value of 1.")]
    float hitValue;

    [SerializeField]
    [Range(0, -1)]
    [Tooltip("This value decides how impactful a miss will be. A perfect gives a value of 1.")]
    float missPenalty;

    [HideInInspector]
    public List<int> beats;

    [HideInInspector]
    public int okayRange;
    [HideInInspector]
    public int perfectRange;

    int index = 0;  /*Index increases only if a hit has been detected, or if a note is not hit at all.
    It does not increase if a note is hit outside of the okay/perfect ranges. 
    This is done so that mass-tapping does not end the corruption prematurely.*/

    [HideInInspector]
    bool inCorruption = false;
    [HideInInspector]
    bool firstBeat = false;

    AudioDistortion audioDistortion;
    AudioManager audioManager;

    [Header("Visual input for drums? Add it here. If not, don't worry, this shouldn't break anything!")]
    [SerializeField] private VisualInput visualInput;

    private OverallCorruption overallCorruption;

    void Start()
    {
        audioManager = GameManager.Instance.audioManager;
        audioDistortion = GameManager.Instance.audioDistortion;
        drumMechanic = GameManager.Instance.drumMechanic.GetComponent<DrumMechanic>();
        overallCorruption = GameManager.Instance.overallCorruption;
        if (visualInput != null)
            visualInput = GameObject.FindGameObjectWithTag("VisualInput").GetComponent<VisualInput>();      // Has to be found by tag unfortunately, but still has to be known that we want to use it, so has to be set in prefab

        for (int i = 0; i < beats.Count; i++)
        {
            beats[i] += duration.start;
        }

        // Set the drum segments to the recording type 'DRUMS'
        overallCorruption.durations[segmentID].recordingType = Duration.RecordingType.DRUMS;

        Assert.IsNotNull(audioManager.gameObject, "Audiomanager not found. Please add an Audiomanager to the game manager.");
        Assert.IsTrue(okayRange >= 0 && perfectRange >= 0, "The DrumCorruption script will not work properly with a negative okayRange or perfectRange");
        Assert.IsTrue(missPenalty <= 0, "Setting missPenalty to anything higher than 0 will reward the player for missing the beat.");
        Assert.IsTrue(hitValue >= 0, "Setting hitvalue to anything lower than 0 will punish the player for hitting the beat.");
    }

    void Update() //All of everything gets done in Update.
    {
        if (audioManager.GetTimeLinePosition() >= duration.start &&
            audioManager.GetTimeLinePosition() < duration.stop && corruptionClearedPercent < clearThreshold) //If player is inside a corrupted area
        {
            if (inSegment == false) //If player just entered the segment
            {
                EnterSegment();
            }

            SetKickMute();

            if (drumMechanic.recording) //If recording
            {
                RecordBeats();
            }
        }
        else if (inSegment) //If player leaves corrupted area
        {
            ExitSegment();
        }
    }

    public override void EnterSegment()
    {
        drumMechanic.gaveInput = false;
        inSegment = true;

        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
        base.EnterSegment();
    }

    public override void ExitSegment()
    {
        inSegment = false;
        if (drumMechanic.recording)
            GradeScore();
        innerDistortion = 0;
        base.ExitSegment();
        ResetConditions();
    }

    void SetKickMute()
    {
        if (audioManager.GetTimeLinePosition() > beats[0] && audioManager.GetTimeLinePosition() < beats[beats.Count - 1] && firstBeat == false)
        { //If the player has just entered the corruption (not the segment)
            audioManager.gameMusicEv.setParameterValue("kick_mute", 1);
            firstBeat = true;
        }
        else if (audioManager.GetTimeLinePosition() > beats[beats.Count - 1] && firstBeat)
        {//If the player has just exited the corruption (not the segment)
            audioManager.gameMusicEv.setParameterValue("kick_mute", 0);
            firstBeat = false;
        }
    }

    void RecordBeats()
    {
        if (index < beats.Count) //If there are more beats available for the player to hit
        {
            if (inCorruption && audioManager.GetTimeLinePosition() > beats[index] + okayRange) //If player missed a beat
            {
                completedBeats.Add(CheckTiming()); //Add the missed beat to the completedBeats list
                index++;
            }
            else if (drumMechanic.gaveInput) //If player gave input
            {
                drumMechanic.gaveInput = false;
                if (!inCorruption)
                    ResetConditions(); //If this is the first input read in the corruption, reset the conditions before proceeding with the rest.
                completedBeats.Add(CheckTiming()); //Add the beat from the player input to the completedBeats list

                inCorruption = true;
            }
        }
    }

    public override void GradeScore()
    {
        corruptionClearedPercent = 0;
        foreach (Timing beat in completedBeats)
        {
            if (beat == Timing.perfect) // Calculate corruption percentage
                corruptionClearedPercent += perfectHitValue / beats.Count;
            else if (beat == Timing.okay)
                corruptionClearedPercent += hitValue / beats.Count;
            else if (beat == Timing.miss)
                corruptionClearedPercent += missPenalty / beats.Count;
        }
        if (corruptionClearedPercent < 0)
            corruptionClearedPercent = 0;

        corruptionClearedPercent *= 100;
    }

    Timing CheckTiming()
    {
        if (audioManager.GetTimeLinePosition() >= beats[index] - okayRange && audioManager.GetTimeLinePosition() <= beats[index] + okayRange)
        { //If within range to hit the beat
            if (audioManager.GetTimeLinePosition() >= beats[index] - perfectRange && audioManager.GetTimeLinePosition() <= beats[index] + perfectRange)
            { //If within range to hit the beat "perfectly"
                Debug.Log("Perfect, " + (float)(audioManager.GetTimeLinePosition()) * 110f / 60000f);
                index++; //Index increases if a note was hit
                if (visualInput != null)
                    visualInput.ChangeColorOnTiming(Timing.perfect);
                return Timing.perfect;
            }
            Debug.Log("Okay, " + (float)(audioManager.GetTimeLinePosition()) * 110f / 60000f);
            index++;//Index increases if a note was hit
            if (visualInput != null)
                visualInput.ChangeColorOnTiming(Timing.okay);
            return Timing.okay;
        }
        Debug.Log("Miss, " + (float)(audioManager.GetTimeLinePosition()) * 110f / 60000f);
        if (visualInput != null)
            visualInput.ChangeColorOnTiming(Timing.miss);
        return Timing.miss;
    }

    void ResetConditions()
    {
        completedBeats.Clear();
        inCorruption = false;
        index = 0;
    }
}
