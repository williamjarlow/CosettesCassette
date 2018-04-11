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

    DrumCorruptionHandler drumCorruptionHandler;

    [Header("Value/Penalty per hit/miss")]
    [Tooltip("Hitting a perfect gives a constant value of 1. Changing this will break the corruption calculations: if (beat == Timing.perfect){ corruptionClearedPercent += perfectHitValue / beats.Count;}")]
    [SerializeField]
    [Range(1, 1)]
    float perfectHitValue = 1;

    [Tooltip("This value decides how impactful an okay will be. A perfect gives a value of 1.")]
    [SerializeField]
    [Range(0, 1)]
    float hitValue;
    
    [Tooltip("This value decides how impactful a miss will be. A perfect gives a value of 1.")]
    [SerializeField]
    [Range(0, -1)]
    float missPenalty;

    [HideInInspector]
    public List<int> beats;
    [HideInInspector]
    public int okayRange;
    [HideInInspector]
    public int perfectRange;
    [HideInInspector]
    public float maxDistortion;

    int index = 0;  /*Index increases only if a hit has been detected, or if a note is not hit at all.
    It does not increase if a note is hit outside of the okay/perfect ranges. 
    This is done so that mass-tapping does not end the corruption prematurely.*/

    bool inCorruption = false;

    AudioDistortion audioDistortion;
    AudioManager audioManager;

	void Start () {
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
        audioDistortion = GameObject.FindWithTag("AudioManager").GetComponent<AudioDistortion>();
        drumCorruptionHandler = GameObject.FindWithTag("CorruptionHandler").GetComponent<DrumCorruptionHandler>();

        Assert.IsNotNull(audioManager.gameObject, "Audiomanager not found. Please add an Audiomanager to the scene and ensure that it is properly tagged.");
        Assert.IsTrue(okayRange >= 0 && perfectRange >= 0, "The DrumCorruption script will not work properly with a negative okayRange or perfectRange");
        Assert.IsTrue(missPenalty <= 0, "Setting missPenalty to anything higher than 0 will reward the player for missing the beat.");
        Assert.IsTrue(hitValue >= 0, "Setting hitvalue to anything lower than 0 will punish the player for hitting the beat.");
	}
	
	void Update () {
        if(Input.GetKeyDown(KeyCode.X)) //Debug button
            Debug.Log(audioManager.GetTimeLinePosition());

        if (audioManager.GetTimeLinePosition() >= duration.start &&
            audioManager.GetTimeLinePosition() < duration.stop) //If player is inside a corrupted area
        {
            if (inCorruption == false) //If player just entered corruption
            {
                drumCorruptionHandler.UpdateCorruptionAmount();
                innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100)); //Set distortion
                drumCorruptionHandler.UpdateDistortionAmount();
                inCorruption = true;
            }

            if (index < beats.Count) //If there are more beats available for the player to hit
            {
                if (corruptionStarted && audioManager.GetTimeLinePosition() > beats[index] + okayRange) //If player missed a beat
                {
                    completedBeats.Add(CheckTiming()); //Add the missed beat to the completedBeats list
                    index++;
                }
                else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Z)) //If player gave input
                {
                    if (!corruptionStarted)
                        ResetConditions(); //If this is the first input read in the corruption, reset the conditions before proceeding with the rest.
                    completedBeats.Add(CheckTiming()); //Add the beat from the player input to the completedBeats list
                    corruptionStarted = true;
                }
            }
        }
        else if (inCorruption == true && (audioManager.GetTimeLinePosition() > duration.stop || audioManager.GetTimeLinePosition() < duration.stop)) //If player leaves corrupted area
        {
            innerDistortion = 0;
            GradeScore();
            inCorruption = false;
            Debug.Log("Percent of corruption cleared: " + corruptionClearedPercent + "%");
            ResetConditions();
        }
	}

    void GradeScore()
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
        drumCorruptionHandler.UpdateCorruptionAmount();
        drumCorruptionHandler.UpdateDistortionAmount();
    }

    Timing CheckTiming()
    {
        if (audioManager.GetTimeLinePosition() >= beats[index] - okayRange && audioManager.GetTimeLinePosition() <= beats[index] + okayRange)
        { //If within range to hit the beat
            if (audioManager.GetTimeLinePosition() >= beats[index] - perfectRange && audioManager.GetTimeLinePosition() <= beats[index] + perfectRange)
            { //If within range to hit the beat "perfectly"
                Debug.Log("Perfect");
                index++; //Index increases if a note was hit
                return Timing.perfect;
            }
            Debug.Log("Okay");
            index++;//Index increases if a note was hit
            return Timing.okay;
        }
        Debug.Log("Miss");
        return Timing.miss;
    }

    void ResetConditions()
    {
        completedBeats.Clear();
        corruptionStarted = false;
        index = 0;
    }
}
