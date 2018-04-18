using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CorruptionBaseClass : MonoBehaviour {
    [HideInInspector] public Duration duration;
    [HideInInspector] public float corruptionClearedPercent;
    [HideInInspector] public float innerDistortion;

    public virtual void EnterCorruption()
    {
        GameManager.Instance.overallCorruption.UpdateCorruptionAmount();
        GameManager.Instance.overallCorruption.UpdateDistortionAmount();
    }
    public virtual void ExitCorruption()
    {
        GameManager.Instance.overallCorruption.UpdateCorruptionAmount();
        GameManager.Instance.overallCorruption.UpdateDistortionAmount();
    }
    public virtual void GradeScore()
    {

    }
}
