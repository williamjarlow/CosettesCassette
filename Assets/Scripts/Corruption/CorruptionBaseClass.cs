using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CorruptionBaseClass : MonoBehaviour {
    [HideInInspector]
    public Duration duration;
    [HideInInspector]
    public float corruptionClearedPercent;
}
