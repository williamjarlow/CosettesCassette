using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Timing{
    miss,
    okay,
    perfect
};

public class DrumCorruption : CorruptionBaseClass {
    List <Timing> completedBeats = new List<Timing>();
    bool corruptionStarted;

    [SerializeField]
    [Range(0, 1)]
    float hitValue;
   const float perfectHitValue = 1;
    [SerializeField]
    [Range(0, -1)]
    float missPenalty;

    [HideInInspector]
    public List<int> beats;
    [HideInInspector]
    public int okayRange;
    [HideInInspector]
    public int perfectRange;

    int index = 0;

    AudioManager audioManager;
	void Start () {
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.X))
            Debug.Log(audioManager.GetTimeLinePosition());

        if (audioManager.GetTimeLinePosition() >= duration.start &&
            audioManager.GetTimeLinePosition() < duration.stop &&
            index < beats.Count)
        {
            if (corruptionStarted && audioManager.GetTimeLinePosition() > beats[index] + okayRange)
            {
                completedBeats.Add(CheckTiming());
                index++;
            }

            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log(audioManager.GetTimeLinePosition());
                completedBeats.Add(CheckTiming());
                corruptionStarted = true;
            }
        }
        else if (corruptionStarted == true && audioManager.GetTimeLinePosition() >= duration.stop)
        {
            if (index < beats.Count)
            {
                for (int i = beats.Count - completedBeats.Count; i < beats.Count; i++)
                {
                    completedBeats.Add(Timing.miss);
                }
            }
            GradeScore();
            Debug.Log(corruptionClearedPercent);
            ResetConditions();
        }
	}

    void GradeScore()
    {
        corruptionClearedPercent = 0;
        foreach (Timing beat in completedBeats)
        {
            if (beat == Timing.perfect)
                corruptionClearedPercent += perfectHitValue / beats.Count;
            else if (beat == Timing.okay)
                corruptionClearedPercent += hitValue / beats.Count;
            else if (beat == Timing.miss)
                corruptionClearedPercent += missPenalty / beats.Count;
        }
        if (corruptionClearedPercent < 0)
            corruptionClearedPercent = 0;
    }

    Timing CheckTiming()
    {
        if (audioManager.GetTimeLinePosition() >= beats[index] - okayRange && audioManager.GetTimeLinePosition() <= beats[index] + okayRange)
        {
            if (audioManager.GetTimeLinePosition() >= beats[index] - perfectRange && audioManager.GetTimeLinePosition() <= beats[index] + perfectRange)
            {
                Debug.Log("Perfect");
                index++;
                return Timing.perfect;
            }
            Debug.Log("Okay");
            index++;
            return Timing.okay;
        }
        Debug.Log("Miss");
        return Timing.miss;
    }

    void ResetConditions()
    {
        completedBeats.Clear();
        corruptionStarted = false;
        index = 0;
    }
}
