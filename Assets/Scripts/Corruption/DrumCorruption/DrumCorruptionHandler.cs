﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[System.Serializable]
public class DrumInformation : CorruptionInformation{
    [Header("Beat corruption placement.")] [Tooltip("16 = 1 bar. 4 = perfectly on beat.")]
    public List<int> beats;

    [Header("Ranges in milliseconds")]
    public int perfectRange;
    public int okayRange;
}

public class DrumCorruptionHandler : CorruptionHandlerBaseClass {

    [SerializeField] [Tooltip("Amount of drum corruptions, as well as their information.")]
    List<DrumInformation> drumInformationList;
    
    [SerializeField] [Tooltip("Select the drumCorruptionPrefab object from the 'Prefabs' folder.")]
    GameObject drumCorruptionPrefab;

    OverallCorruption overallCorruption;

    void Awake () {

        if (drumInformationList.Count <= 0)
            Assert.IsNotNull(drumCorruptionPrefab);

        overallCorruption = GetComponent<OverallCorruption>();
        Assert.IsNotNull(overallCorruption, "Please make sure that the DrumCorruptionHandler script is on the same object as the OverallCorruption script.");

		foreach(DrumInformation drumInformation in drumInformationList) //Set starting values for corruption
        {
            GameObject go = Instantiate(drumCorruptionPrefab, gameObject.transform);
            DrumCorruption drumCorruption = go.GetComponent<DrumCorruption>();
            corruptions.Add(drumCorruption);
            drumCorruption.beats = drumInformation.beats;
            drumCorruption.perfectRange = drumInformation.perfectRange;
            drumCorruption.okayRange = drumInformation.okayRange;
            drumCorruption.maxDistortion = drumInformation.maxDistortion;
            drumCorruption.clearThreshold = drumInformation.clearThreshold;
        }
	}
    void Start()
    {
        foreach (DrumInformation drumInformation in drumInformationList) //Set starting values for corruption
        {
            foreach (DrumCorruption drumCorruption in corruptions)
            {
                for (int i = 0; i < drumInformation.beats.Count; i++)
                {
                    drumCorruption.beats[i] = drumInformation.beats[i] * overallCorruption.bpmInMs;
                }
                drumCorruption.duration = overallCorruption.durations[drumInformation.segmentID];
            }
        }
    }
}
