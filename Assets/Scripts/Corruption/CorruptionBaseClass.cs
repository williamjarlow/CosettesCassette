using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CorruptionBaseClass : MonoBehaviour {
    [HideInInspector] public Duration duration;
    [HideInInspector] public float corruptionClearedPercent;
    [HideInInspector] public float innerDistortion;

    public virtual void EnterSegment()
    {
        GameManager.Instance.overallCorruption.UpdateCorruptionAmount();
        GameManager.Instance.overallCorruption.UpdateDistortionAmount();
    }
    public virtual void ExitSegment()
    {
        GameManager.Instance.overallCorruption.UpdateCorruptionAmount();
        GameManager.Instance.overallCorruption.UpdateDistortionAmount();
    }
    public virtual void GradeScore()
    {

    }
}
