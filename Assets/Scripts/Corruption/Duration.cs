using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Duration
{
    public int start;
    public int stop;

    private List<int> drumRecordings = new List<int>();

    // List with two values per index
    private List<KeyValuePair<int, float>> pitchRecordings = new List<KeyValuePair<int, float>>();

    // Recording type to differentiate the corrupted area
    public enum RecordingType { DRUMS, PITCH };
    public RecordingType recordingType { get; set; }


    // Add more checks when adding more mechanics
    public bool HasRecordings()
    {
        if (drumRecordings.Count == 0 && pitchRecordings.Count == 0)
            return false;

        else
            return true;
    }

    public void AddDrumRecordings(int timeStamp)
    {
        drumRecordings.Add(timeStamp);
    }

    public List<int> GetDrumRecordings()
    {
        return drumRecordings;
    }

    public void AddPitchRecordings(int timeStamp, float pitchSliderValue)
    {
        pitchRecordings.Add(new KeyValuePair<int, float>(timeStamp, pitchSliderValue));
    }

    public List<KeyValuePair<int, float>> GetPitchRecordings()
    {
        return pitchRecordings;
    }

    public void ClearRecordings()
    {
        drumRecordings.Clear();
        pitchRecordings.Clear();
    }


}