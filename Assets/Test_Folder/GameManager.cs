using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : Singleton<GameManager> {

    public AudioDistortion audioDistortion;
    public AudioManager audioManager;
    public OverallCorruption overallCorruption;
    public List<int> segments;
    public int bpm;


    [HideInInspector] public int bpmInMs;

   [HideInInspector] public List<Duration> durations;

    [HideInInspector] public float pitch;
    [HideInInspector] public float posInSong;
    [HideInInspector] public float lengthOfSong;

	void Awake ()
    {
        Assert.AreNotEqual(bpm, 0, "BPM cannot be 0.");

        bpmInMs = ConvertBpmToMs(bpm);
        for (int i = 0; i < segments.Count; i++)
        {
            Duration duration = new Duration();
            segments[i] = segments[i] * bpmInMs;
            if (i == 0)
                duration.start = 0;
            else
                duration.start = segments[i - 1];

            duration.stop = segments[i];
            durations.Add(duration);
        }
    }

	void Update ()
    {

	}

    public float GetPosInSong()
    {
        return audioManager.GetTimeLinePosition();
    }

    public int ConvertBpmToMs(int bpm)
    {
        return Mathf.RoundToInt((60000 / 4) / bpm);
    }
}
