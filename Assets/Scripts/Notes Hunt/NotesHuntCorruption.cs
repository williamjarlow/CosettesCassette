﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// ** Written by Hannes Gustafsson and William Jarlow ** // 

public class NotesHuntCorruption : CorruptionBaseClass
{
    [System.Serializable]
    public class Notes
    {
        public NoteType noteType;
        [Tooltip("Spawntime in milliseconds relative to the song")] public int spawnTime;
        [HideInInspector] public int points;  // The points that the player recieves when destroying the object. Public to be able to use multipliers for powerups, etc
        [HideInInspector] public bool hasSpawned;
    }

    private AudioManager audioManager;
    private OverallCorruption overallCorruption;
    [SerializeField] private GameObject correctNotePrefab;
    [SerializeField] private GameObject incorrectNotePrefab;
    [Tooltip("Time tolerance in ms when comparing timeline position and recorded beats")] private int tolerance = 30;
    private float maxPoints;
    private float currentPoints;
    [SerializeField] private int correctNotePoints;
    [SerializeField] private int incorrectNotePoints;
    [SerializeField] private float speed;
    [Tooltip("Minimum x spawn coordinate")] [SerializeField] private float xSpawnRandomMin;
    [Tooltip("Maximum x spawn coordinate")] [SerializeField] private float xSpawnRandomMax;
    [SerializeField] private float timeStamp;
    private float spawnCooldown = 0.05f;

    public enum NoteType { CORRECT, INCORRECT };
    // The list of notes the designers will spawn
    public List<Notes> notesList;

                // ** TODO ** //
    // 1. Fix corruption calculation in ExitSegment()

    void Start()
    {
        audioManager = GameManager.Instance.audioManager;
        overallCorruption = GameManager.Instance.overallCorruption;

        duration = overallCorruption.durations[segmentID];

        // Initialize points
        correctNotePrefab.GetComponent<NoteMovement>().points = correctNotePoints;
        incorrectNotePrefab.GetComponent<NoteMovement>().points = incorrectNotePoints;

        for (int i = 0; i < notesList.Count; i++)
        {
            if (notesList[i].noteType == NoteType.CORRECT)
            {
                notesList[i].points = correctNotePoints;
                maxPoints += notesList[i].points;
            }
                

            else if (notesList[i].noteType == NoteType.INCORRECT)
            {
                notesList[i].points = incorrectNotePoints;
            }
                
        }
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

        // ** Notes Hunt ** //
        timeStamp = GameManager.Instance.audioManager.GetTimeLinePosition();

        // If we are in the current segment
        if (inSegment)
        {
            SpawnNotes();

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

                // If an object was hit
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    // Add/remove points
                    currentPoints += hit.transform.gameObject.GetComponent<NoteMovement>().points;
                    Destroy(hit.transform.gameObject);
                }
            }

            // ** Temporary for testing ** // 
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // If an object was hit
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    // Add/remove points
                    currentPoints += hit.transform.gameObject.GetComponent<NoteMovement>().points;
                    Destroy(hit.transform.gameObject);
                }
            }
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
        //if (GameManager.Instance.recording)
            GradeScore();
        innerDistortion = 0;
        base.ExitSegment();
        ResetConditions();
    }

    public override void GradeScore()
    {
        if((currentPoints / maxPoints) > clearThreshold)
            corruptionClearedPercent = 100;

        else
        {
            corruptionClearedPercent = (currentPoints / maxPoints) * 100;
        }
    }

    void ResetConditions()
    {
        //You can use this function to reset any conditions that need to be reset upon leaving a segment.

    }

    private void SpawnNotes()
    {
        // Loop through the list of notes
        for (int i = 0; i < notesList.Count; i++)
        {
            // If we are at the right time stamp (with some tolerance) --> spawn the note according to the object's note type
            if(timeStamp > 0 && timeStamp <= notesList[i].spawnTime + tolerance / 2 && timeStamp >= notesList[i].spawnTime - tolerance / 2)
            {
                GameObject spawnedNote;

                if (notesList[i].noteType == NoteType.CORRECT && !notesList[i].hasSpawned)
                {
                    // Spawn the correct note and set it's speed
                    notesList[i].hasSpawned = true;
                    spawnedNote = Instantiate(correctNotePrefab, new Vector3(Random.Range(xSpawnRandomMin, xSpawnRandomMax), 0), Quaternion.identity);
                    spawnedNote.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.up * speed);
                }

                else if(notesList[i].noteType == NoteType.INCORRECT && !notesList[i].hasSpawned)
                {
                    // Spawn the correct note and set it's speed
                    notesList[i].hasSpawned = true;
                    spawnedNote = Instantiate(incorrectNotePrefab, new Vector3(Random.Range(xSpawnRandomMin, xSpawnRandomMax), 0), Quaternion.identity);
                    spawnedNote.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.up * speed);
                }
            }
        }
    }

    private IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(spawnCooldown);
        SpawnNotes();
    }
}
