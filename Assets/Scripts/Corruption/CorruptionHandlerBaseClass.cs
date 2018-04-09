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
}
