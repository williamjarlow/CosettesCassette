using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CorruptionTemplate : CorruptionBaseClass
{
    AudioManager audioManager;
    OverallCorruption overallCorruption;

    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
        overallCorruption = gameManager.overallCorruption;
        audioManager = gameManager.audioManager;
        duration = overallCorruption.durations[segmentID];
    }

    void Update()
    {
        if (audioManager.GetTimeLinePosition() >= duration.start &&
            audioManager.GetTimeLinePosition() < duration.stop) //If player is inside a corrupted segment
        {
            if (gameManager.recording) //If recording
            {
                if (inSegment == false) //If player just entered the segment
                {
                    EnterSegment();
                }
            }
        }
        else if (inSegment) //If player leaves the segment area
        {
            ExitSegment();
        }
    }

    public override void EnterSegment()
    { 
        //This function gets called upon when entering the segment
        inSegment = true;
        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
        if (gameManager.recording)
            corruptionClearedPercent = 0;
        base.EnterSegment();
    }

    public override void ExitSegment()
    {
        //This function gets called upon when leaving the segment
        inSegment = false;
        if (gameManager.recording)
            GradeScore(); //Score gets evaluated and saved to file here.
        corruptionClearedPercent = Mathf.Clamp(corruptionClearedPercent, 0, 100);
        innerDistortion = 0;
        base.ExitSegment();
        ResetConditions();
    }

    public override void GradeScore() 
    {
        //Here's where you want to set your corruptionClearPercent to reflect how well the player did.
        base.GradeScore(); //Score gets evaluated and saved to file here.
    }

    void ResetConditions()
    {
        //You can use this function to reset any conditions that need to be reset upon leaving a segment.
    }
}
