using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : Singleton<GameManager> {

    [HideInInspector] public AudioDistortion audioDistortion;
    [HideInInspector] public OverallCorruption overallCorruption;
    [HideInInspector] public AudioPitch audioPitch;
    public GameObject drumMechanic;
    public AudioManager audioManager;
    public GameObject corruptionHandler;
    public GameObject timelineSlider;
    [SerializeField] List<int> segmentEnds;
    [HideInInspector] public List<int> segments;
    [Range(1, 200)] public int bpm;

    [HideInInspector] public int bpmInMs;

    [HideInInspector] public List<Duration> durations;

    [HideInInspector] public float pitch;
    [HideInInspector] public float posInSong;
    [HideInInspector] public float lengthOfSong;

    

	void Awake ()
    {
        audioDistortion = audioManager.GetComponent<AudioDistortion>();
        audioPitch = audioManager.GetComponent<AudioPitch>();
        overallCorruption = corruptionHandler.GetComponent<OverallCorruption>();

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

    private void Start()
    {
        Debug.Assert(this.gameObject.tag == "GameManager", "Set GameManager tag to GameManager");
    }


    void Update ()
    {

	}

    int ConvertBpmToMs(int bpm)
    {
        return Mathf.RoundToInt((60000 / 4) / bpm);
    }
}
