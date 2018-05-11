using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// ** Written by Hannes Gustafsson ** // 

public class NotesHuntCorruption : CorruptionBaseClass
{

                // ** DISCLAIMER ** //
    //  Curently the 'points' variable of the class 'Notes' has no use. Might be reworked in the future

    [System.Serializable]
    public class Notes
    {
        public NoteType noteType;
        [Tooltip("Spawntime in milliseconds relative to the song")] public int spawnTime;
        [HideInInspector] public int points;  // The points that the player recieves when destroying the object. Public to be able to use multipliers for powerups, etc
        [HideInInspector] public bool hasSpawned;
    }


    public enum NoteType { CORRECT, INCORRECT };
    public enum NoteValue { SINGLE, DOUBLE };

    private AudioManager audioManager;
    private OverallCorruption overallCorruption;

    [SerializeField] private GameObject correctNotePrefab;
    [SerializeField] private GameObject incorrectNotePrefab;
    [Tooltip("The values added to the note box collider to compensate for its size")][SerializeField] private Vector2 addedBoxColliderSize = new Vector2(0.6f, 0.2f);
    [SerializeField] private Sprite[] correctNotesSprites = new Sprite[5];
    [SerializeField] private Sprite[] incorrectNotesSprites = new Sprite[5];
    private int noteSpriteIndex;    // Index to use when randomzing the sprite for the note
    [Tooltip("Time tolerance in ms when comparing timeline position and recorded beats")] private int tolerance = 30;
    [SerializeField] private int correctNotePoints;
    [SerializeField] private int incorrectNotePoints;
    [SerializeField] private int amountOfNoteValues = 2;    // The amount of note values, for now double and single which equals to 2
    private float maxScore;
    private float spawnCooldown = 0.05f;
    [SerializeField] private float speed;
    [Tooltip("Minimum x spawn coordinate")] [SerializeField] private float xSpawnRandomMin;
    [Tooltip("Maximum x spawn coordinate")] [SerializeField] private float xSpawnRandomMax;
    [SerializeField] private float timeStamp;
    

    // The list of notes the designers will spawn
    public List<Notes> notesList;
    // The list of notes (the game object) to destroy when exiting segment
    [HideInInspector] public List<GameObject> destroyList;

    void Start()
    {
        audioManager = GameManager.Instance.audioManager;
        overallCorruption = GameManager.Instance.overallCorruption;

        duration = overallCorruption.durations[segmentID];


        // Initialize points
        correctNotePrefab.GetComponent<NoteMovement>().points = correctNotePoints;
        incorrectNotePrefab.GetComponent<NoteMovement>().points = incorrectNotePoints;

        // Set points and calculate max points
        for (int i = 0; i < notesList.Count; i++)
        {
            if (notesList[i].noteType == NoteType.CORRECT)
            {
                notesList[i].points = correctNotePoints;
                maxScore += notesList[i].points;
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
            if (GameManager.Instance.recording) //If recording
            {
                if (inSegment == false) //If player just entered the segment
                {
                    EnterSegment();
                }
                RecordSegment();
            }
        }
        else if (inSegment) //If player leaves the segment area
        {
            ExitSegment();
        }
    }

    private void RecordSegment()
    {
        // ** Notes Hunt ** //
        timeStamp = GameManager.Instance.audioManager.GetTimeLinePosition();
        SpawnNotes();
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            // If an object was hit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // Add/remove points and destroy the hit object
                currentScore += hit.transform.gameObject.GetComponent<NoteMovement>().points;
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
                // Add/remove points and destroy the hit object
                currentScore += hit.transform.gameObject.GetComponent<NoteMovement>().points;
                Destroy(hit.transform.gameObject);
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

                // Randomize the index of the sprite list. Since both sprite lists should be the same size it should not matter which list is chosen 
                noteSpriteIndex = Random.Range(0, correctNotesSprites.Length - 1);

                // Correct note
                if (notesList[i].noteType == NoteType.CORRECT && !notesList[i].hasSpawned)
                {
                    //Randomize the sprite
                    noteSpriteIndex = Random.Range(0, correctNotesSprites.Length - 1);

                    // Spawn the correct randomized note
                    notesList[i].hasSpawned = true;
                    spawnedNote = Instantiate(correctNotePrefab, new Vector3(Random.Range(xSpawnRandomMin, xSpawnRandomMax), 0), Quaternion.identity);
                    // Set the sprite according to the randomly generated sprite
                    spawnedNote.GetComponent<SpriteRenderer>().sprite = correctNotesSprites[noteSpriteIndex];
                    // Set the game object's speed according to the specified speed in this script
                    spawnedNote.GetComponent<NoteMovement>().speed = speed;

                    // Save the box collider and sprite as variables
                    BoxCollider boxCol = spawnedNote.GetComponent<BoxCollider>();
                    Sprite sprite = spawnedNote.GetComponent<SpriteRenderer>().sprite;

                    // Convert the sprite to world space units
                    float pixelsPerUnit = sprite.pixelsPerUnit;
                    // Set the box collider size according to the world space size + the added box collider size
                    boxCol.size = new Vector3(sprite.rect.size.x / pixelsPerUnit + addedBoxColliderSize.x, sprite.rect.size.y / pixelsPerUnit + addedBoxColliderSize.y, 0);

                    destroyList.Add(spawnedNote);
                }

                // Incorrect note
                else if (notesList[i].noteType == NoteType.INCORRECT && !notesList[i].hasSpawned)
                {
                    //Randomize the sprite
                    noteSpriteIndex = Random.Range(0, incorrectNotesSprites.Length - 1);

                    // Spawn the correct randomized note
                    notesList[i].hasSpawned = true;
                    spawnedNote = Instantiate(incorrectNotePrefab, new Vector3(Random.Range(xSpawnRandomMin, xSpawnRandomMax), 0), Quaternion.identity);
                    // Set the sprite according to the randomly generated sprite
                    spawnedNote.GetComponent<SpriteRenderer>().sprite = incorrectNotesSprites[noteSpriteIndex];
                    // Set the game object's speed according to the specified speed in this script
                    spawnedNote.GetComponent<NoteMovement>().speed = speed;

                    // Save the box collider and sprite as variables
                    BoxCollider boxCol = spawnedNote.GetComponent<BoxCollider>();
                    Sprite sprite = spawnedNote.GetComponent<SpriteRenderer>().sprite;

                    // Convert the sprite to world space units
                    float pixelsPerUnit = sprite.pixelsPerUnit;
                    // Set the box collider size according to the world space size + the added box collider size
                    boxCol.size = new Vector3(sprite.rect.size.x / pixelsPerUnit + addedBoxColliderSize.x, sprite.rect.size.y / pixelsPerUnit + addedBoxColliderSize.y, 0);

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
    }

    public override void ExitSegment()
    {
        //This function gets called upon when leaving the segment
        inSegment = false;
        GradeScore();
        innerDistortion = 0;

        base.ExitSegment();

        // Reset conditions, i.e destroy notes to improve performance
        ResetConditions();
    }

    public override void GradeScore()
    {
        currentScore = currentScore / maxScore;
        base.GradeScore();
    }

    void ResetConditions()
    {
        // Loop through the list of notes and set the hasSpawned bool to false. Then destroy all notes.
        for(int i = 0; i < notesList.Count; i++)
        {
            notesList[i].hasSpawned = false;
        }

        DestroyNotes();
    }
}

