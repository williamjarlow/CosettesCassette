using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CorruptionBaseClass : MonoBehaviour {
    public Duration duration;
    [HideInInspector] public float corruptionClearedPercent;
    [HideInInspector] public float innerDistortion;

    public virtual void EnterCorruption()
    {

    }
    public virtual void ExitCorruption()
    {

    }
    public virtual void UpdateCorruption()
    {

    }

}
