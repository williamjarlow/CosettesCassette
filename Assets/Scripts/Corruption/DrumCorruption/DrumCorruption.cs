using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum Timing{
    miss,
    okay,
    perfect
};

public class DrumCorruption : CorruptionBaseClass {
    List <Timing> completedBeats = new List<Timing>();
    bool corruptionStarted;

    DrumMechanic drumMechanic;

    DrumCorruptionHandler drumCorruptionHandler;

    [SerializeField] [Range(1, 1)] [Header("Value/Penalty per hit/miss")] [Tooltip("Hitting a perfect gives a constant value of 1. Changing this will break the corruption calculations: if (beat == Timing.perfect){ corruptionClearedPercent += perfectHitValue / beats.Count;}")]
    float perfectHitValue = 1;

    [SerializeField] [Range(0, 1)] [Tooltip("This value decides how impactful an okay will be. A perfect gives a value of 1.")]
    float hitValue;
    
    [SerializeField] [Range(0, -1)] [Tooltip("This value decides how impactful a miss will be. A perfect gives a value of 1.")]
    float missPenalty;

    [HideInInspector] public List<int> beats;

    [HideInInspector] public int okayRange;
    [HideInInspector] public int perfectRange;

    [HideInInspector] public float maxDistortion;

    int index = 0;  /*Index increases only if a hit has been detected, or if a note is not hit at all.
    It does not increase if a note is hit outside of the okay/perfect ranges. 
    This is done so that mass-tapping does not end the corruption prematurely.*/

   [HideInInspector] public bool inCorruption = false;

    AudioDistortion audioDistortion;
    AudioManager audioManager;

	void Start () {
        audioManager = GameManager.Instance.audioManager;
        audioDistortion = GameManager.Instance.audioDistortion;
        drumMechanic = GameObject.Find("DrumMechanic").GetComponent<DrumMechanic>();

        Assert.IsNotNull(audioManager.gameObject, "Audiomanager not found. Please add an Audiomanager to the game manager.");
        Assert.IsTrue(okayRange >= 0 && perfectRange >= 0, "The DrumCorruption script will not work properly with a negative okayRange or perfectRange");
        Assert.IsTrue(missPenalty <= 0, "Setting missPenalty to anything higher than 0 will reward the player for missing the beat.");
        Assert.IsTrue(hitValue >= 0, "Setting hitvalue to anything lower than 0 will punish the player for hitting the beat.");
	}
	
	void Update () {
        if (audioManager.GetTimeLinePosition() >= duration.start &&
            audioManager.GetTimeLinePosition() < duration.stop) //If player is inside a corrupted area
        {
            if (inCorruption == false) //If player just entered corruption
            {
                EnterCorruption();
            }
            if (drumMechanic.recording) //If recording
            {
                if (index < beats.Count) //If there are more beats available for the player to hit
                {
                    if (corruptionStarted && audioManager.GetTimeLinePosition() > beats[index] + okayRange) //If player missed a beat
                    {
                        completedBeats.Add(CheckTiming()); //Add the missed beat to the completedBeats list
                        index++;
                    }
                    else if (drumMechanic.gaveInput) //If player gave input
                    {
                        drumMechanic.gaveInput = false;
                        if (!corruptionStarted)
                            ResetConditions(); //If this is the first input read in the corruption, reset the conditions before proceeding with the rest.
                        completedBeats.Add(CheckTiming()); //Add the beat from the player input to the completedBeats list
                        corruptionStarted = true;
                    }
                }
            }
        }
        else if (inCorruption) //If player leaves corrupted area
        {
            ExitCorruption();
        }
	}

    public override void EnterCorruption()
    {
        
        drumMechanic.gaveInput = false;
        audioManager.gameMusicEv.setParameterValue("kick_mute", 1);
        inCorruption = true;

        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
        base.EnterCorruption();
    }

    public override void ExitCorruption()
    {
        inCorruption = false;
        audioManager.gameMusicEv.setParameterValue("kick_mute", 0);
        if (drumMechanic.recording)
            GradeScore();
        innerDistortion = 0;
        base.ExitCorruption();
        ResetConditions();
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
                Debug.Log("Perfect, " +  audioManager.GetTimeLinePosition());
                index++; //Index increases if a note was hit
                return Timing.perfect;
            }
            Debug.Log("Okay, " + audioManager.GetTimeLinePosition());
            index++;//Index increases if a note was hit
            return Timing.okay;
        }
        Debug.Log("Miss, " + audioManager.GetTimeLinePosition());
        return Timing.miss;
    }

    void ResetConditions()
    {
        completedBeats.Clear();
        corruptionStarted = false;
        index = 0;
    }
}
