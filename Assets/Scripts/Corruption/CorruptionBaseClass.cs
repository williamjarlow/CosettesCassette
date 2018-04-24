using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CorruptionBaseClass : MonoBehaviour {
    [HideInInspector] public Duration duration;
    [HideInInspector] public float corruptionClearedPercent;
    [HideInInspector] public float innerDistortion;
    [HideInInspector] public float maxDistortion;
    [HideInInspector] public bool inSegment;

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
