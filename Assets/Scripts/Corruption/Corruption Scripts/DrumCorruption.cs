using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Timing{
    miss,
    hit,
    perfect
};

public class DrumCorruption : CorruptionBaseClass {
    // Use this for initialization
    [SerializeField]
    List<int> beats;
    [SerializeField]
    float perfectRange;
    [SerializeField]
    float okayRange;
    Dictionary<int, Timing> completedBeats;

    int index = 0;

    AudioManager audioManager;
	void Start () {
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if(audioManager.GetTimeLinePosition() >= duration.start && audioManager.GetTimeLinePosition() < duration.stop)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                completedBeats.Add(beats[index], CheckTiming());
                index++;
            }
        }
	}

    Timing CheckTiming()
    {
        if (audioManager.GetTimeLinePosition() >= beats[index] - okayRange || audioManager.GetTimeLinePosition() <= beats[index] + okayRange)
        {
            if (audioManager.GetTimeLinePosition() >= beats[index] - perfectRange || audioManager.GetTimeLinePosition() <= beats[index] + perfectRange)
            {
                return Timing.perfect;
            }
            return Timing.hit;
        }
        return Timing.miss;
    }

}
