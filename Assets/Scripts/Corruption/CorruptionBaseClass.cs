using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class CorruptionBaseClass : MonoBehaviour
{

    [Range(0, 100)]
    public float maxDistortion;
    [HideInInspector]
    public float currentDistortion;
    [Range(0, 100)]
    [Tooltip("Percentage of corruption that has to be beaten in order for the segment to be 'cleared'")]
    public int clearThreshold;
    [Range(0, 100)]
    [Tooltip("Percentage of corruption that has to be beaten in order for the segment to be 'perfect-ed'")]
    public int perfectThreshold;
    [Header("ID of segment in 'Game Manager'")]
    public int segmentID;
    [HideInInspector]
    public float bestScore = 0;
    [HideInInspector]
    public float currentScore = 0;

    [HideInInspector]
    public GameManager gameManager;

    private SaveSegmentStruct saveStruct;

    [TextArea]
    [SerializeField]
    string notes;

    [HideInInspector]
    public Duration duration;
    [HideInInspector]
    public float corruptionClearedPercent;
    [HideInInspector]
    public float innerDistortion;
    [HideInInspector]
    public bool inSegment;
    [HideInInspector]
    public bool cleared;
    [HideInInspector]
    public bool perfected;


    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
        saveStruct = new SaveSegmentStruct();
    }

    public virtual void EnterSegment()
    {
        inSegment = true;
        gameManager.overallCorruption.UpdateCorruptionAmount();
        gameManager.overallCorruption.UpdateDistortionAmount();
    }
    public virtual void ExitSegment()
    {
        inSegment = false;
        gameManager.overallCorruption.UpdateCorruptionAmount();
        gameManager.overallCorruption.UpdateDistortionAmount();

        //This means that if player hits record the record button won't be disabled.
        if (gameManager.audioManager.GetTimeLinePosition() >= duration.start)
        {
            // Set recording to false and set the minigame button position to up and set the playbutton to interactable
            gameManager.SetRecord(false);
            gameManager.minigameButton.SetPositionUp();
            gameManager.playButton.interactable = true;
        }
    }
    public virtual void GradeScore()
    {
        if (currentScore > bestScore)
        {
            bestScore = currentScore;

            if (bestScore >= perfectThreshold)
            {
                bestScore = 100; //Perfect Segment
				gameManager.audioManager.PlaySegmentClear(1f);
                cleared = true;
                perfected = true;
            }
            else if (bestScore >= clearThreshold)
            {
                //Cleared Segment
                cleared = true;
                gameManager.audioManager.PlaySegmentClear(0f);
            }
        }
        currentScore = 0;
        corruptionClearedPercent = bestScore;
        Save();
    }

    public virtual void Save()
    {
        saveStruct.points = corruptionClearedPercent;
        saveStruct.exists = true;
        SaveSystem.Instance.SaveSegment(saveStruct, SceneManager.GetActiveScene().buildIndex, segmentID);
    }

    public virtual void Load()
    {
        saveStruct = SaveSystem.Instance.LoadSegment(saveStruct, SceneManager.GetActiveScene().buildIndex, segmentID);
        corruptionClearedPercent = saveStruct.points;
        bestScore = saveStruct.points;
        //gameManager.overallCorruption.UpdateCorruptionAmount();
    }
}