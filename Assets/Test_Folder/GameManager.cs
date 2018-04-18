using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : Singleton<GameManager> {

    [HideInInspector] public AudioDistortion audioDistortion;
    [HideInInspector] public OverallCorruption overallCorruption;
    public GameObject drumMechanic;
    public AudioManager audioManager;
    public GameObject corruptionHandler;
    [SerializeField] List<int> segmentEnds;
    [HideInInspector] public List<Segment> segments;
    [Range(1, 200)] public int bpm;

    [HideInInspector] public int bpmInMs;

   [HideInInspector] public List<Duration> durations;

    [HideInInspector] public float pitch;
    [HideInInspector] public float posInSong;
    [HideInInspector] public float lengthOfSong;

	void Awake ()
    {
        foreach(int segmentEnd in segmentEnds)
        {
            Segment segment = new Segment();
            segment.segmentEnd = segmentEnd;
            segments.Add(segment);
        }

        audioDistortion = audioManager.GetComponent<AudioDistortion>();
        overallCorruption = corruptionHandler.GetComponent<OverallCorruption>();

        bpmInMs = ConvertBpmToMs(bpm);
        for (int i = 0; i < segments.Count; i++)
        {
            Duration duration = new Duration();
            segments[i].segmentEnd = segments[i].segmentEnd * bpmInMs;
            if (i == 0)
                duration.start = 0;
            else
                duration.start = segments[i - 1].segmentEnd;

            duration.stop = segments[i].segmentEnd;
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
