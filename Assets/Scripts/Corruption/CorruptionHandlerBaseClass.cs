using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Duration
{
    public int start;
    public int stop;
}

public class CorruptionHandlerBaseClass : MonoBehaviour {
    [HideInInspector]
    public Duration duration;

    public virtual float GetCorruptionAmount()
    {
        return 0;
    }

    public virtual float GetDistortionAmount()
    {
        return 0; 
    }

    public virtual void UpdateDistortionAmount()
    {

    }

    public virtual void UpdateCorruptionAmount()
    {

    }
}
