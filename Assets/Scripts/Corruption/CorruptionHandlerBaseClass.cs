using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Duration
{
    public int start;
    public int stop;
    
    private List<int> drumRecordings = new List<int>();

    public void AddDrumRecordings(int timeStamp)
    {
        drumRecordings.Add(timeStamp);
    }

    public List<int> GetDrumRecordings()
    {
        return drumRecordings;
    }

    public void ClearRecordings()
    {
        drumRecordings.Clear();
    }
}

public class CorruptionHandlerBaseClass : MonoBehaviour {
    [HideInInspector] public float distortionAmount;
    [HideInInspector] public float corruptionAmount;
    [HideInInspector] public List<CorruptionBaseClass> corruptions;
}
