using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeBas : MonoBehaviour {
    bool corruptionstarted;

    [Header("Damage and Gain per segment")]
    private int Perfectrange;
    [SerializeField] [Range(0,1)] private int missDamage;
    [SerializeField] [Range(0,1)]private int hitGain;

    [Header("SuccessRanges")]
    public int okayRange;
    public int perfectRange;

    public List<int> Segment;
    DrumCorruptionHandler drumCorruptionHandler;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
