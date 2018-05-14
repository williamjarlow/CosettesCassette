using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

// ** Written by Hannes Gustafsson, template by William Jarlow ** //

public class OddOneOutCorruption : CorruptionBaseClass
{
    private AudioManager audioManager;
    private OverallCorruption overallCorruption;
    private GameObject lyricsObject;    // Object to be instantiated
    [SerializeField] private GameObject lyricsPrefab;
    private List<GameObject> lyricPages = new List<GameObject>();
    //[SerializeField] private List<string> lyrics = new List<string>();
    [SerializeField] private string[] lyrics = new string[3];
    [SerializeField] private int correctLyricSegment;
    private bool hasSpawned;


                                // ** TODO ** // 
        // 1. Add some visual effect when clicking on a button and destroying the lyrics
    void Start()
    {
        overallCorruption = GameManager.Instance.overallCorruption;
        audioManager = GameManager.Instance.audioManager;
        duration = overallCorruption.durations[segmentID];
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

        if(inSegment && GameManager.Instance.recording && hasSpawned == false && !GameManager.Instance.audioManager.switchedToAudioLog)
        {
            SpawnLyrics();
        }
    }

    private void SpawnLyrics()
    {
        hasSpawned = true;

        Debug.Log("Spawned lyrics");

        // Instantiate prefab
        lyricsObject = Instantiate(lyricsPrefab, GameManager.Instance.uiParent.transform);

        // ** Initialize ** //
        for (int i = 0; i < lyrics.Length; i++)
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

    private void CorrectLyricChoice()
    {
        // Add some visual effect
        DestroyLyrics();

        // The player clicked the correct button --> segment is fully cleared
        currentScore = 100;
        GradeScore();
        
    }

    private void IncorrectLyricChoice()
    {
        // Add some visual effect
        DestroyLyrics();

        // The player clicked the incorrect button --> set score to 0 and add some distortion?
        currentScore = 0;
        GradeScore();
    }

    private void DestroyLyrics()
    {
        Destroy(lyricsObject);
        lyricPages.Clear();
    }

    public override void EnterSegment()
    {
        ResetConditions();
        inSegment = true;
        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
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
        base.GradeScore();
    }

    private void ResetConditions()
    {
        // Destroy the lyric objects and enable spawning of the lyric objects
        DestroyLyrics();
        hasSpawned = false;
    }


}
