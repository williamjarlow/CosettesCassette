using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class OddOneOutCorruption : CorruptionBaseClass
{
    AudioManager audioManager;
    OverallCorruption overallCorruption;
    [SerializeField]
    List<string> lyrics = new List<string>();
    [SerializeField]
    GameObject lyricPagePrefab;
    List<GameObject> lyricPageInstances = new List<GameObject>();

    void Start()
    {
        overallCorruption = GameManager.Instance.overallCorruption;
        audioManager = GameManager.Instance.audioManager;
        duration = overallCorruption.durations[segmentID];
        clearThreshold = 99;
    }

    void Update()
    {
        if (audioManager.GetTimeLinePosition() >= duration.start &&
            audioManager.GetTimeLinePosition() < duration.stop) //If player is inside a corrupted segment
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
        for (int i = 0; i < lyrics.Count; i++)
        {
            lyricPageInstances.Add(Instantiate(lyricPagePrefab, gameObject.transform));
            lyricPageInstances[i].GetComponent<Text>().text = lyrics[i];
        }
        inSegment = true;
        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
        if (GameManager.Instance.recording)
            corruptionClearedPercent = 0;
        base.EnterSegment();
    }

    public override void ExitSegment()
    {
        //This function gets called upon when leaving the segment
        inSegment = false;
        foreach (GameObject lyricPage in lyricPageInstances)
            Destroy(lyricPage);
        if (GameManager.Instance.recording)
            GradeScore();
        corruptionClearedPercent = Mathf.Clamp(corruptionClearedPercent, 0, 100);
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
