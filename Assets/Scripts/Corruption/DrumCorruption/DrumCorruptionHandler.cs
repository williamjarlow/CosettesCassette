using System.Collections;
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

    public bool autoGenerateCorruption;

    [Header("ID of segment in 'Game Manager'")]
    public int segmentID;
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

            if (drumInformation.autoGenerateCorruption)
                GenerateCorruption(drumInformation);

            for (int i = 0; i < drumInformation.beats.Count; i++)
            {
                drumCorruption.beats[i] = drumInformation.beats[i] * overallCorruption.bpmInMs;
            }
            drumCorruption.duration = overallCorruption.durations[drumInformation.segmentID];
        }
	}
    void GenerateCorruption(DrumInformation drumInformation)
    {
        drumInformation.beats.Clear();
        for (int i = 0; i < overallCorruption.segments.Count; i++)
        {
            if (i == drumInformation.segmentID - 1)
            {
                for (int j = overallCorruption.segments[i].start; j < overallCorruption.segments[i].stop; j += 4)
                {
                    drumInformation.beats.Add(j);
                }
            }
        }
    }
}
