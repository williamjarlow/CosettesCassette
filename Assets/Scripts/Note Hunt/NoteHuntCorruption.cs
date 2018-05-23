using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// ** Written by Hannes Gustafsson ** // 

public enum NoteType { CORRECT, INCORRECT, CORRECTMULTI };
public enum NoteValue { SINGLE, DOUBLE };

public class NoteHuntCorruption : CorruptionBaseClass
{

    // ** DISCLAIMER ** //
    //  Curently the 'points' variable of the class 'Notes' has no use. Might be reworked in the future

    [System.Serializable]
    public class Notes
    {
        public NoteType noteType;
        public MovementType movementType;
        [Tooltip("Spawntime in milliseconds relative to the song")]
        public int spawnTime;
        [HideInInspector]
        public int points;  // The points that the player recieves when destroying the object. Public to be able to use multipliers for powerups, etc
        [HideInInspector]
        public bool hasSpawned;
    }

    private AudioManager audioManager;
    private OverallCorruption overallCorruption;
    [SerializeField]
    GameObject noteExplosionPrefab;

    [SerializeField]
    float noteExplosionTime;
    [SerializeField]
    bool spawnNotesAtEdge; //Debug bool for spawning notes at edge values.
    [SerializeField]
    private GameObject correctNotePrefab;
    [SerializeField]
    private GameObject correctMultiNotePrefab;
    [SerializeField]
    private GameObject incorrectNotePrefab;
    [Tooltip("The values added to the note box collider to compensate for its size")]
    [SerializeField]
    private Vector2 addedBoxColliderSize = new Vector2(0.6f, 0.2f);
    [SerializeField]
    private Sprite[] correctNotesSprites = new Sprite[3];
    [SerializeField]
    private Sprite[] correctMultiNotesSprites = new Sprite[2];
    [SerializeField]
    private Sprite[] incorrectNotesSprites = new Sprite[5];
    private int noteSpriteIndex;    // Index to use when randomzing the sprite for the note
    [Tooltip("Time tolerance in ms when comparing timeline position and recorded beats")]
    private int tolerance = 30;
    [SerializeField]
    private int correctNotePoints;
    [SerializeField]
    private int correctMultiNotePoints;
    [SerializeField]
    private int incorrectNotePoints;

    private float maxScore;
    private float spawnCooldown = 0.05f;
    [SerializeField]
    [Range(0, 0.1f)]
    private float speed;
    [Tooltip("Minimum x spawn coordinate")]
    [SerializeField]
    private float xSpawnRandomMin;
    [Tooltip("Maximum x spawn coordinate")]
    [SerializeField]
    private float xSpawnRandomMax;

    [SerializeField]
    [Tooltip("Check this box if input is in bpm rather than milliseconds")]
    private bool inputBPM;

    const int standardNoteHits = 1;
    const int multiNoteHits = 2;

    float timeStamp;

    // The list of notes the designers will spawn
    public List<Notes> noteList;
    // The list of notes (the game object) to destroy when exiting segment
    [HideInInspector]
    public List<GameObject> destroyList;
    void Start()
    {
        audioManager = gameManager.audioManager;
        overallCorruption = gameManager.overallCorruption;

        duration = overallCorruption.durations[segmentID];

        // Initialize points
        correctNotePrefab.GetComponent<NoteMovement>().points = correctNotePoints;
        correctMultiNotePrefab.GetComponent<NoteMovement>().points = correctMultiNotePoints;
        incorrectNotePrefab.GetComponent<NoteMovement>().points = incorrectNotePoints;

        // Set points and calculate max points
        for (int i = 0; i < noteList.Count; i++)
        {
            if (inputBPM)
                noteList[i].spawnTime *= gameManager.overallCorruption.bpmInMs;
            if (noteList[i].noteType == NoteType.CORRECT)
            {
                noteList[i].points = correctNotePoints;
                maxScore += noteList[i].points;
            }
            else if (noteList[i].noteType == NoteType.CORRECTMULTI)
            {
                noteList[i].points = correctMultiNotePoints;
                maxScore += noteList[i].points;
            }
            else if (noteList[i].noteType == NoteType.INCORRECT)
            {
                noteList[i].points = incorrectNotePoints;
            }

        }
        Load();
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
        timeStamp = Mathf.Clamp(gameManager.audioManager.GetTimeLinePosition() - duration.start, 0, duration.stop - duration.start);
        SpawnNotes();
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray;
            if (Input.GetMouseButtonDown(0))
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            else
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            // If an object was hit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                NoteMovement noteMovement = hit.transform.gameObject.GetComponent<NoteMovement>();
                // If we hit the note
                if (noteMovement != null)
                {
                    // Add/remove points and destroy the hit object
                    if (noteMovement.hitsRemaining <= 1)
                    {
                        //Give audio/visual feedback for destroying note
                        currentScore += noteMovement.points;
                        GameObject noteExplosioninstance;
                        noteExplosioninstance = Instantiate(noteExplosionPrefab, hit.transform.position, hit.transform.rotation, transform);
                        Destroy(hit.transform.gameObject);
                        Destroy(noteExplosioninstance, noteExplosionTime);

						if (noteMovement.points < 0) // correct note
							audioManager.PlayShootSound (1f);
						else if (noteMovement.points > 0) // incorrect note
							audioManager.PlayShootSound (0f);
                    }
                    else
                    {
                        //Give audio/visual feedback for hitting note
                        noteMovement.hitsRemaining -= 1;
                        GameObject noteExplosioninstance;
                        noteExplosioninstance = Instantiate(noteExplosionPrefab, hit.transform.position, hit.transform.rotation, transform);
                        Destroy(noteExplosioninstance, noteExplosionTime);
                        int i = Random.Range(0, 3);
                        hit.transform.gameObject.GetComponent<SpriteRenderer>().sprite = correctNotesSprites[i];
                        audioManager.PlayShootSound(2f);
                    }
                }
            }
        }
    }

    private void SpawnNotes()
    {
        // Loop through the list of notes
        for (int i = 0; i < noteList.Count; i++)
        {
            // If we are at the right time stamp (with some tolerance) --> spawn the note according to the object's note type
            if (timeStamp > 0 && timeStamp <= noteList[i].spawnTime + tolerance / 2 && timeStamp >= noteList[i].spawnTime - tolerance / 2)
            {
                GameObject spawnedNote;

                // Correct note
                if (noteList[i].noteType == NoteType.CORRECT && !noteList[i].hasSpawned)
                {
                    //Randomize the sprite
                    noteSpriteIndex = Random.Range(0, correctNotesSprites.Length - 1);
                    //Debug for spawning notes at edges
                    if (spawnNotesAtEdge)
                        if (Random.Range(0, 2) == 0)
                            spawnedNote = Instantiate(correctNotePrefab, new Vector3(xSpawnRandomMin, transform.localPosition.y, transform.localPosition.z), Quaternion.identity, transform);
                        else
                            spawnedNote = Instantiate(correctNotePrefab, new Vector3(xSpawnRandomMax, transform.localPosition.y, transform.localPosition.z), Quaternion.identity, transform);
                    else
                        spawnedNote = Instantiate(correctNotePrefab, new Vector3(Random.Range(xSpawnRandomMin, xSpawnRandomMax), transform.localPosition.y, transform.localPosition.z), Quaternion.identity, transform);
                    // Set the sprite according to the randomly generated sprite
                    spawnedNote.GetComponent<SpriteRenderer>().sprite = correctNotesSprites[noteSpriteIndex];
                    // Set the required amount of hits
                    spawnedNote.GetComponent<NoteMovement>().hitsRemaining = standardNoteHits;
                }

                // Incorrect note
                else if (noteList[i].noteType == NoteType.INCORRECT && !noteList[i].hasSpawned)
                {
                    //Randomize the sprite
                    noteSpriteIndex = Random.Range(0, incorrectNotesSprites.Length - 1);
                    //Debug for spawning notes at edges
                    if (spawnNotesAtEdge)
                        if (Random.Range(0, 2) == 0)
                            spawnedNote = Instantiate(incorrectNotePrefab, new Vector3(xSpawnRandomMin, transform.localPosition.y, transform.localPosition.z), Quaternion.identity, transform);
                        else
                            spawnedNote = Instantiate(incorrectNotePrefab, new Vector3(xSpawnRandomMax, transform.localPosition.y, transform.localPosition.z), Quaternion.identity, transform);
                    else
                        spawnedNote = Instantiate(incorrectNotePrefab, new Vector3(Random.Range(xSpawnRandomMin, xSpawnRandomMax), transform.localPosition.y, transform.localPosition.z), Quaternion.identity, transform);
                    // Set the sprite according to the randomly generated sprite
                    spawnedNote.GetComponent<SpriteRenderer>().sprite = incorrectNotesSprites[noteSpriteIndex];
                    // Set the required amount of hits
                    spawnedNote.GetComponent<NoteMovement>().hitsRemaining = standardNoteHits;
                }
                else if (noteList[i].noteType == NoteType.CORRECTMULTI && !noteList[i].hasSpawned)
                {
                    //Randomize the sprite
                    noteSpriteIndex = Random.Range(0, correctMultiNotesSprites.Length - 1);
                    //Debug for spawning notes at edges
                    if (spawnNotesAtEdge)
                        if (Random.Range(0, 2) == 0)
                            spawnedNote = Instantiate(correctMultiNotePrefab, new Vector3(xSpawnRandomMin, transform.localPosition.y, transform.localPosition.z), Quaternion.identity, transform);
                        else
                            spawnedNote = Instantiate(correctMultiNotePrefab, new Vector3(xSpawnRandomMax, transform.localPosition.y, transform.localPosition.z), Quaternion.identity, transform);
                    else
                        spawnedNote = Instantiate(correctMultiNotePrefab, new Vector3(Random.Range(xSpawnRandomMin, xSpawnRandomMax), transform.localPosition.y, transform.localPosition.z), Quaternion.identity, transform);
                    // Set the sprite according to the randomly generated sprite
                    spawnedNote.GetComponent<SpriteRenderer>().sprite = correctMultiNotesSprites[noteSpriteIndex];
                    // Set the required amount of hits
                    spawnedNote.GetComponent<NoteMovement>().hitsRemaining = multiNoteHits;
                }
                else
                {
                    spawnedNote = new GameObject();
                    Destroy(spawnedNote);
                }
                if (!noteList[i].hasSpawned)
                {
                    // Spawn the correct randomized note
                    noteList[i].hasSpawned = true;
                    // Set the game object's speed according to the specified speed in this script
                    spawnedNote.GetComponent<NoteMovement>().speed = speed;
                    // Save the box collider and sprite as variables
                    BoxCollider boxCol = spawnedNote.GetComponent<BoxCollider>();
                    Sprite sprite = spawnedNote.GetComponent<SpriteRenderer>().sprite;
                    // Convert the sprite to world space units
                    float pixelsPerUnit = sprite.pixelsPerUnit;
                    // Set the box collider size according to the world space size + the added box collider size
                    boxCol.size = new Vector3(sprite.rect.size.x / pixelsPerUnit + addedBoxColliderSize.x, sprite.rect.size.y / pixelsPerUnit + addedBoxColliderSize.y, 0);

                    // Prepare the object for destruction
                    destroyList.Add(spawnedNote);
                    // Set note movement type
                    spawnedNote.GetComponent<NoteMovement>().movementType = noteList[i].movementType;
                }
            }
        }
    }

    private void DestroyNotes()
    {
        for (int i = 0; i < destroyList.Count; i++)
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
        // Reset the conditions, i.e destroy notes to be able to replay and reset
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
        // Reset conditions, i.e destroy notes to improve performance
        ResetConditions();
        base.ExitSegment();
    }

    public override void GradeScore()
    {
        currentScore = currentScore * 100 / maxScore;
        base.GradeScore();
    }

    void ResetConditions()
    {
        // Loop through the list of notes and set the hasSpawned bool to false. Then destroy all notes.
        for (int i = 0; i < noteList.Count; i++)
        {
            noteList[i].hasSpawned = false;
        }
        DestroyNotes();
    }
}
