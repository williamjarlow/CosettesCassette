using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum eventTriggered {jackDamaged, passedEnemy};

[System.Serializable]
class runnerEnemies
{
    public int spawnTime;
    [HideInInspector]public bool hasSpawned = false;
}

public class JackTheRunner : CorruptionBaseClass
{
    AudioManager audioManager;
    OverallCorruption overallCorruption;

    [SerializeField] private GameObject[] enemiesPrefabs;
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject jackTheRunnerPrefab;
    [SerializeField] private List<runnerEnemies> enemiesList;

    private int maxScore = 0;
    private int damageTaken = 0;
    private float timeStamp = 0;
    private float tolerance = 30;


    void Start()
    {
        for (int i = 0; i < enemiesList.Count; i++)
        {
            enemiesList[i].hasSpawned = false;
            enemiesList[i].spawnTime *= gameManager.overallCorruption.bpmInMs;
            maxScore += 1;
        }
        damageTaken = 0;

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
                RecordSegment();
            }

            else if (!gameManager.recording)
                ResetConditions();
        }

        else if (inSegment) //If player leaves the segment area
        {
            ExitSegment();
        }
    }

    private void RecordSegment()
    {

        timeStamp = Mathf.Clamp(gameManager.audioManager.GetTimeLinePosition() - duration.start, 0, duration.stop - duration.start);
        SpawnEnemies();
    }

    public override void EnterSegment()
    {
        ResetConditions();
        inSegment = true;
        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
        if (gameManager.recording)
            corruptionClearedPercent = 0;
        Instantiate(groundPrefab, transform);
        Instantiate(jackTheRunnerPrefab, transform);
        base.EnterSegment();
    }

    public override void ExitSegment()
    {

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
        currentScore = ((maxScore - damageTaken) * 100) / maxScore;
        base.GradeScore(); //Score gets evaluated and saved to file here.
    }

    void ResetConditions()
    {
        for (int i = 0; i < enemiesList.Count; i++)
        {
            enemiesList[i].hasSpawned = false;
        }
        currentScore = 0;
        damageTaken = 0;
        DestroyObjects();
    }

    private void SpawnEnemies()
    {
        // Loop through the list of enemies
        for (int i = 0; i < enemiesList.Count; i++)
        {
            // If we are at the right time stamp (with some tolerance) --> spawn enemy
            if (timeStamp > 0 && timeStamp <= enemiesList[i].spawnTime + tolerance / 2 && timeStamp >= enemiesList[i].spawnTime - tolerance / 2)
            {
                if (!enemiesList[i].hasSpawned)
                {
                    enemiesList[i].hasSpawned = true;
                    Instantiate(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Length)], transform);
                }
            }
        }
    }

    public void RegisterEvent(eventTriggered whatHappened)
    {
        if (whatHappened == eventTriggered.jackDamaged)
        {
            damageTaken += 1;
        }
    }

    private void DestroyObjects()
    {
        int i = 0;

        //Array to hold all child objects
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Find all child objects and store to that array
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Destroy children
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
