using System.Collections;
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
    // The list of notes (the game object) to destroy when exiting segment
    public List<GameObject> destroyList;

                // ** TODO ** //
    // 1. Fix being able to replay the segment
    // 2. Save the best attempt

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
        if (inSegment && GameManager.Instance.recording)
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
                    // Spawn the correct note
                    notesList[i].hasSpawned = true;
                    spawnedNote = Instantiate(correctNotePrefab, new Vector3(Random.Range(xSpawnRandomMin, xSpawnRandomMax), 0), Quaternion.identity);
                    spawnedNote.GetComponent<NoteMovement>().speed = speed;
                    destroyList.Add(spawnedNote);
                }

                else if(notesList[i].noteType == NoteType.INCORRECT && !notesList[i].hasSpawned)
                {
                    // Spawn the correct note
                    notesList[i].hasSpawned = true;
                    spawnedNote = Instantiate(incorrectNotePrefab, new Vector3(Random.Range(xSpawnRandomMin, xSpawnRandomMax), 0), Quaternion.identity);
                    spawnedNote.GetComponent<NoteMovement>().speed = speed;
                    destroyList.Add(spawnedNote);
                }
            }
        }
    }

    private void DestroyNotes()
    {
        for(int i = 0; i < destroyList.Count; i++)
        {
            if (destroyList[i] != null)
                Destroy(destroyList[i].transform.gameObject);
        }

        destroyList.Clear();
    }

    private IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(spawnCooldown);
        SpawnNotes();
    }

    public override void EnterSegment()
    {
        // Reset the conditions, i.e destroy notes to be able to replay
        ResetConditions();

        inSegment = true;
        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
        base.EnterSegment();

        Debug.Log("enter segment ");
    }

    public override void ExitSegment()
    {
        //This function gets called upon when leaving the segment
        inSegment = false;
        GradeScore();
        innerDistortion = 0;

        DestroyNotes();

        base.ExitSegment();

        // Reset conditions, i.e destroy notes to improve performance
        ResetConditions();
    }

    public override void GradeScore()
    {
        if ((currentPoints / maxPoints) > clearThreshold)
            corruptionClearedPercent = 100;

        else
        {
            corruptionClearedPercent = (currentPoints / maxPoints) * 100;
        }
    }

    void ResetConditions()
    {
        //You can use this function to reset any conditions that need to be reset upon leaving a segment.
        for(int i = 0; i < notesList.Count; i++)
        {
            notesList[i].hasSpawned = false;
        }

        DestroyNotes();
    }
}

