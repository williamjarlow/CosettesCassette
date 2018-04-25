using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Duration
{
    public int start;
    public int stop;

    private List<int> drumRecordings = new List<int>();
    private List<int> pitchRecordings = new List<int>();

    // Recording type to differentiate the corrupted area
    public enum RecordingType { DRUMS, PITCH };
    public RecordingType recordingType { get; set; }
    
    public void AddDrumRecordings(int timeStamp)
    {
        drumRecordings.Add(timeStamp);
    }

    public List<int> GetDrumRecordings()
    {
        return drumRecordings;
    }

    public void AddPitchRecordings(int timeStamp)
    {
        pitchRecordings.Add(timeStamp);
    }

    public List<int> GetPitchRecordings()
    {
        return pitchRecordings;
    }

    public void ClearRecordings()
    {
        drumRecordings.Clear();
        pitchRecordings.Clear();
    }


}

public class CorruptionHandlerBaseClass : MonoBehaviour {
    [HideInInspector] public float distortionAmount;
    [HideInInspector] public float corruptionAmount;
    [HideInInspector] public List<CorruptionBaseClass> corruptions;
}
