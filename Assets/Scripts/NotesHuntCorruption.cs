using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NotesHuntCorruption : CorruptionBaseClass
{
    AudioManager audioManager;

    void Start()
    {
        audioManager = GameManager.Instance.audioManager;
    }

    void Update()
    {
        if (audioManager.GetTimeLinePosition() >= duration.start &&
            audioManager.GetTimeLinePosition() < duration.stop &&
            corruptionClearedPercent < clearThreshold) //If player is inside a corrupted segment
        {
            if (inSegment == false) //If player just entered the segment
            {
                EnterSegment();
            }

            if (GameManager.Instance.recording) //If recording
            {
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
        base.EnterSegment();
    }

    public override void ExitSegment()
    {
        //This function gets called upon when leaving the segment
        inSegment = false;
        if (GameManager.Instance.recording)
            GradeScore();
        innerDistortion = 0;
        base.ExitSegment();
        ResetConditions();
    }

    public override void GradeScore()
    {
        corruptionClearedPercent = 100;
    }

    void ResetConditions()
    {
        //You can use this function to reset any conditions that need to be reset upon leaving a segment.
    }
}

