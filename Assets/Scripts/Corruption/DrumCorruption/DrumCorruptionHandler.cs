using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class DrumInformation{
    [Header("Beat corruption placement.")] [Tooltip("16 = 1 bar. 4 = perfectly on beat.")]
    public List<int> beats;

    [Header("Ranges in milliseconds")]
    public int perfectRange;
    public int okayRange;

    Duration duration;
    [Header("ID of segment in 'Game Manager'")]
    public int segmentID;

    [Range(0, 100)] public float maxDistortion;
    [HideInInspector] public float currentDistortion;
}

public class DrumCorruptionHandler : CorruptionHandlerBaseClass {

    [SerializeField] int bpm;
    float bpmConverted;

    [SerializeField] [Tooltip("Amount of drum corruptions, as well as their information.")]
    List<DrumInformation> drumInformationList;
    
    [SerializeField] [Tooltip("Select the drumCorruptionPrefab object from the 'Prefabs' folder.")]
    GameObject drumCorruptionPrefab;

    List<DrumCorruption> drumCorruptions = new List<DrumCorruption>();

    OverallCorruption overallCorruption;

    void Awake()
    {
        if(drumInformationList.Count <= 0)
            Assert.IsNotNull(drumCorruptionPrefab);
    }

    void Start () {

        bpmConverted = (60000/4) / bpm; //Convert bpm into milliseconds

        overallCorruption = GetComponent<OverallCorruption>();
        Assert.IsNotNull(overallCorruption, "Please make sure that the DrumCorruptionHandler script is on the same object as the OverallCorruption script.");

		foreach(DrumInformation drumInformation in drumInformationList) //Set starting values for corruption
        {
            GameObject go = Instantiate(drumCorruptionPrefab, gameObject.transform);
            DrumCorruption drumCorruption = go.GetComponent<DrumCorruption>();
            drumCorruptions.Add(drumCorruption);
            drumCorruption.beats = drumInformation.beats;
            drumCorruption.perfectRange = drumInformation.perfectRange;
            drumCorruption.okayRange = drumInformation.okayRange;
            drumCorruption.maxDistortion = drumInformation.maxDistortion;
            for (int i = 0; i < drumInformation.beats.Count; i++)
            {
                drumCorruption.beats[i] = Mathf.RoundToInt(drumInformation.beats[i] * bpmConverted);
            }
            //Set corruption duration to segment value based on drumInformation.segment

        }
	}

	void Update () {

	}

    public override void UpdateDistortionAmount()
    {
        distortionAmount = 0;
        foreach (DrumCorruption drumCorruption in drumCorruptions)
        {
            distortionAmount += drumCorruption.innerDistortion;
        }
        overallCorruption.UpdateDistortionAmount();  
    }

    public override void UpdateCorruptionAmount()
    {
        corruptionAmount = 0;
        foreach (DrumCorruption drumCorruption in drumCorruptions)
        {
                corruptionAmount += (100 - drumCorruption.corruptionClearedPercent) / drumCorruptions.Count;
        }
        overallCorruption.UpdateCorruptionAmount();
    }
}
