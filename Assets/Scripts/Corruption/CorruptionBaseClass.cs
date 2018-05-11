using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class CorruptionBaseClass : MonoBehaviour {

    [Range(0, 100)]
    public float maxDistortion;
    [HideInInspector]
    public float currentDistortion;
    [Range(0, 100)] [Tooltip("Percentage of corruption that has to be beaten in order for the segment to be 'cleared'")]
    public int clearThreshold;
    [Range(0, 100)] [Tooltip("Percentage of corruption that has to be beaten in order for the segment to be 'perfect-ed'")]
    public int perfectThreshold;
    [Header("ID of segment in 'Game Manager'")]
    public int segmentID;
    [HideInInspector]
    public float bestScore = 0;
    [HideInInspector]
    public float currentScore = 0;

    private SaveSegmentStruct saveStruct;

    [TextArea]
    [SerializeField]
    string notes;

    [HideInInspector] public Duration duration;
    [HideInInspector] public float corruptionClearedPercent;
    [HideInInspector] public float innerDistortion;
    [HideInInspector] public bool inSegment;
    [HideInInspector] public bool cleared;

    void Awake()
    {
        saveStruct = new SaveSegmentStruct();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            Load();
        if (Input.GetKeyDown(KeyCode.S))
            Save();
    }

    public virtual void EnterSegment()
    {
        inSegment = true;
        GameManager.Instance.overallCorruption.UpdateCorruptionAmount();
        GameManager.Instance.overallCorruption.UpdateDistortionAmount();
    }
    public virtual void ExitSegment()
    {
        inSegment = false;
        GameManager.Instance.overallCorruption.UpdateCorruptionAmount();
        GameManager.Instance.overallCorruption.UpdateDistortionAmount();
        //GameManager.Instance.recording = false;
    }
    public virtual void GradeScore()
    {
        if (currentScore > bestScore)
        {
            bestScore = currentScore;

            if (bestScore > perfectThreshold)
            {
                bestScore = 100; //Perfect Segment
            }
            else if (bestScore > clearThreshold)
            {
                //Cleared Segment
                GameManager.Instance.audioManager.PlaySegmentClear(0f);
            }
            else
            {
                //New Highscore
            }
        }
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
    }
}
