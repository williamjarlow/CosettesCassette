using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CorruptionBaseClass : MonoBehaviour {

    [Range(0, 100)]
    public float maxDistortion;
    [HideInInspector]
    public float currentDistortion;
    [Range(0, 100)]
    public int clearThreshold;
    [Header("ID of segment in 'Game Manager'")]
    public int segmentID;

    [HideInInspector] public Duration duration;
    [HideInInspector] public float corruptionClearedPercent;
    [HideInInspector] public float innerDistortion;
    [HideInInspector] public bool inSegment;
    [HideInInspector] public bool cleared;

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
    }
    public virtual void GradeScore()
    {

    }
}
