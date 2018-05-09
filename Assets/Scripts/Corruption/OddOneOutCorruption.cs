using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class OddOneOutCorruption : CorruptionBaseClass
{
    private AudioManager audioManager;
    private OverallCorruption overallCorruption;
    [SerializeField] private GameObject lyricsPrefab;
    private List<GameObject> lyricPages = new List<GameObject>();
    [SerializeField] private List<string> lyrics = new List<string>();
    [SerializeField] private int correctLyricSegment;

    void Start()
    {
        overallCorruption = GameManager.Instance.overallCorruption;
        audioManager = GameManager.Instance.audioManager;
        duration = overallCorruption.durations[segmentID];

        SpawnLyrics();

        //GetComponent<Button>().onClick.AddListener(CorrectChoice);
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

    private void SpawnLyrics()
    {
        // Instantiate prefab
        GameObject lyricsObject;
        lyricsObject = Instantiate(lyricsPrefab, GameManager.Instance.uiParent.transform); 

        // ** Initialize ** //
        for (int i = 0; i < lyrics.Count; i++)
        {
            // Find the lyric pages
            lyricPages.Add(lyricsObject.transform.GetChild(i).gameObject);

            // Set the text component to the specified strings
            lyricPages[i].transform.GetChild(0).GetComponent<Text>().text = lyrics[i];

            // Set the correct/incorrect onClick() functions 
            if (i == correctLyricSegment)
                lyricPages[i].GetComponent<Button>().onClick.AddListener(CorrectLyricChoice);

            else
                lyricPages[i].GetComponent<Button>().onClick.AddListener(IncorrectLyricChoice);

        }

        Debug.Assert(lyricPages.Count == lyricsObject.transform.childCount, "The number of lyrics does not equal to the number of lyric pages");

    }

    public void CorrectLyricChoice()
    {
        // Functionality for clicking on the correct text
        Debug.Log("Pressed the correct button");
    }

    public void IncorrectLyricChoice()
    {
        // Functionality for clicking on the incorrect text
        Debug.Log("Pressed the incorrect button");
    }

    public override void EnterSegment()
    {
        //This function gets called upon when entering the segment
       /* for (int i = 0; i < lyrics.Count; i++)
        {
            lyricPageInstances.Add(Instantiate(lyricsPrefab, GameManager.Instance.uiParent.transform) as GameObject);
            lyricPageInstances[i].GetComponent<Text>().text = lyrics[i];
        }*/



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

    private void ResetConditions()
    {
        //You can use this function to reset any conditions that need to be reset upon leaving a segment.

        //Destroy the lyric page instances
        for (int i = 0; i < lyricPages.Count; i++)
        {
            Destroy(lyricPages[i].gameObject);
        }
    }

}
