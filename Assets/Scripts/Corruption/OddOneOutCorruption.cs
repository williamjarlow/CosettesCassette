using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

// ** Written by Hannes Gustafsson, template by William Jarlow ** //

public class OddOneOutCorruption : CorruptionBaseClass
{
    private AudioManager audioManager;
    private OverallCorruption overallCorruption;
    private GameObject lyricsGuessObject;
    private GameObject lyricsObject;    // Object to be instantiated
    [SerializeField] private GameObject lyricsGuessPrefab;
    [SerializeField] private GameObject lyricsPrefab;
    private List<GameObject> lyricPages = new List<GameObject>();
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color correctColor;
    [SerializeField] private Color incorrectColor;
    [SerializeField] private string lyricsGuess;
    [SerializeField] private string[] lyrics = new string[3];
    [SerializeField] private int correctLyricSegment;
    private bool hasSpawned;

    [SerializeField] private float chosenWordStart;
    [SerializeField] private float chosenWordEnd;

    // Timer
    [SerializeField] private GameObject timerPrefab;
    private GameObject timerObject;    // Timer object to be instantiated
    [Tooltip("Timer startpoin in ms/beats from the segment start")][SerializeField] private float timerStartPoint;
    [SerializeField] [Tooltip("Time in seconds the timer will display")] private float timerLength;
    private float originalTimerLength;
    [Tooltip("Check this box if input is in bpm rather than milliseconds")]
    [SerializeField]
    private bool inputBeats;        // Does the designer want to design stuff in beats or milliseconds?
    private bool spawnedTimer;      // Used to check if we have spawned the timer
    private bool displayChoice;     // Used to check if we are displaying the results/choice, i.e which toggle/word was pressed
    private bool resetConditions;   // Used to reset conditions only once

                                // ** TODO ** // 
        // 1. Fix calling ResetConditions() every frame. Happens when inSegment and !recording
    void Start()
    {
        overallCorruption = gameManager.overallCorruption;
        audioManager = gameManager.audioManager;
        duration = overallCorruption.durations[segmentID];

        // If the designer wants to work in beats rather than milliseconds
        if (inputBeats)
            timerStartPoint *= overallCorruption.bpmInMs;

        originalTimerLength = timerLength;

        Load();
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

            if (gameManager.recording) //If recording
            {
               
                if (inSegment && !gameManager.audioManager.switchedToAudioLog)
                {
                    Timer();
                }

                if (inSegment && hasSpawned == false && !gameManager.audioManager.switchedToAudioLog)
                {
                    SpawnLyrics();
                }   

            }

            // If we stopped recording --> destroy the mechanic objects by resetting
            else if (!gameManager.recording && !resetConditions)
            {
                ResetConditions();
                resetConditions = true;
            }

            // If player is currently listening to chosen word
            if (audioManager.GetTimeLinePosition () >= chosenWordStart &&
			    audioManager.GetTimeLinePosition () < chosenWordEnd)
				audioManager.oooVocals.setValue (100f);
			else // If player is not currently listening to chosen word
				audioManager.oooVocals.setValue (30f);
        }

        else if (inSegment) //If player leaves the segment area
        {
            ExitSegment();

            // Reset conditions if we are not recording (ExitSegment sets recording to false)
            // Required to not have the lyrics spawn/despawn when replaying a segment
            if (!gameManager.recording)
            {
                ResetConditions();
            }
                
        }
    }

    private void Timer()
    {
        // Makes the timer relative to the segments, i.e timerStartPoint is x number of beats/milliseconds into the segment
        float timeStamp = Mathf.Clamp(gameManager.audioManager.GetTimeLinePosition() - duration.start, 0, duration.stop - duration.start);

        // Check if we are at the timerStartPoint with some tolerance
        if (timeStamp < timerStartPoint + 20 && timeStamp > timerStartPoint - 20 && !spawnedTimer)
        {
            // Instantiate the timer object
            timerObject = Instantiate(timerPrefab, gameManager.uiParent.transform);
            spawnedTimer = true;
        }

        // If the timer is > 1 and we are not displaying the results, and the music is not paused --> decrement the timer and update the game object
        if (timerLength > 1 && spawnedTimer && !gameManager.audioManager.pausedMusic)
        {
            timerLength -= Time.deltaTime;

            // Convert timerlength to int
            int temp = (int)timerLength;

            // Set the text of the instantiated object to the timer length converted to int
            timerObject.GetComponent<Text>().text = temp.ToString();
        }

        // Destroy the game object when timerLength < 1 and evaluate the chosen lyric
        if (timerLength < 1 && !displayChoice)
        {
            Destroy(timerObject);
            EvaluateToggles();
            displayChoice = true;
        }

    }

    private void SpawnLyrics()
    {
        hasSpawned = true;

        // Instantiate prefabs
        lyricsObject = Instantiate(lyricsPrefab, gameManager.uiParent.transform);
        lyricsGuessObject = Instantiate(lyricsGuessPrefab, gameManager.uiParent.transform);

        // ** Initialize ** //
        lyricsGuessObject.GetComponent<Text>().text = lyricsGuess;

        for (int i = 0; i < lyrics.Length; i++)
        {
            // Find the lyric pages
            lyricPages.Add(lyricsObject.transform.GetChild(i).gameObject);

            // Set the text components to the specified strings
            lyricPages[i].GetComponent<Text>().text = lyrics[i];

            // Add listeners to the toggles to be able to call functions when toggled
            lyricPages[i].GetComponent<Toggle>().onValueChanged.AddListener(delegate { OnToggleValueChanged(); } );
         }

        Debug.Assert(lyricPages.Count == lyricsObject.transform.childCount, "The number of lyrics does not equal to the number of lyric pages");
     }

    private void OnToggleValueChanged()
    {
        // Find the currently selected game object
        GameObject currentEvent = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        
        if(!displayChoice)
        {
            // If we have a toggle selected and we are not displaying the results --> change the color to highlighted
            if (currentEvent.GetComponent<Toggle>().isOn)
            {
                // Reset the colors
                for (int i = 0; i < lyricPages.Count; i++)
                    lyricPages[i].GetComponent<Text>().color = normalColor;

                // Set highlighted color and play sound
                currentEvent.GetComponent<Text>().color = highlightedColor;
                audioManager.PlayOOOSelect();
            }

            // Else set the color to normal
            else
            {
                currentEvent.GetComponent<Text>().color = normalColor;
                audioManager.PlayOOOSelect();
            }
        }

    }

     private void EvaluateToggles()
     {
         // Loop through the toggles, i.e lyric pages
         for (int i = 0; i < lyricPages.Count; i++)
         {
             // Check the selected lyric
             if (lyricPages[i].GetComponent<Toggle>().isOn)
             {
                 // If the correct lyric was chosen --> change color of the text to correctColor and set the score to 100
                 if (i == correctLyricSegment)
                 {
                    lyricPages[i].GetComponent<Text>().color = correctColor;
                    currentScore = 100;
                    GradeScore();
					audioManager.PlayOOOResult (0f);
                 }

                 // If not, the player chose the incorrect lyric --> change color of the text to incorrectColor and set the score to 0
                 else
                 {
                    lyricPages[i].GetComponent<Text>().color = incorrectColor;
                    currentScore = 0;
                    GradeScore();
					audioManager.PlayOOOResult (1f);
                 }

                 // The player clicked the incorrect button --> set score to 0 and add some distortion?
                 currentScore = 0;
                 GradeScore();
             }

         }
     }

     private void DestroyLyrics()
     {
         Destroy(lyricsGuessObject);
         Destroy(lyricsObject);
         lyricPages.Clear();
     }

     public override void EnterSegment()
     {
        inSegment = true;
        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
        base.EnterSegment();

        resetConditions = false;
     }

     public override void ExitSegment()
     {
         //This function gets called upon when leaving the segment
         inSegment = false;
         GradeScore();
         corruptionClearedPercent = Mathf.Clamp(corruptionClearedPercent, 0, 100);
         innerDistortion = 0;
         base.ExitSegment();

         audioManager.oooVocals.setValue (0f);
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

         // Destroy timer object and reset variables
         Destroy(timerObject);
         timerLength = originalTimerLength;
         spawnedTimer = false;

         displayChoice = false;
     }


 }


                     // ** Temporary savings ** //
 /* 
  *    private void CorrectLyricChoice()
     {
         // Add some visual effect
         //DestroyLyrics();

         // Finds the currently selected event, i.e the lyrics game object
         UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().color = correctColor;

         // The player clicked the correct button --> segment is fully cleared
         currentScore = 100;
         GradeScore();

     }

     private void IncorrectLyricChoice()
     {
         // Add some visual effect
         //DestroyLyrics();

         // Finds the currently selected event, i.e the lyrics game object
         UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().color = incorrectColor;

         // The player clicked the incorrect button --> set score to 0 and add some distortion?
         currentScore = 0;
         GradeScore();
     }

                 // Set the correct/incorrect onClick() functions 
             if (i == correctLyricSegment)
                 lyricPages[i].GetComponent<Button>().onClick.AddListener(CorrectLyricChoice);

             else
                 lyricPages[i].GetComponent<Button>().onClick.AddListener(IncorrectLyricChoice);
*/
