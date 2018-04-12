using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class DrumInformation{
    public List<int> beats;
    public int perfectRange;
    public int okayRange;
    public Duration duration;
    [Range(0, 100)]
    public float maxDistortion;
    [HideInInspector]
    public float currentDistortion;
}

public class DrumCorruptionHandler : CorruptionHandlerBaseClass {

    [SerializeField] [Tooltip("Amount of drum corruptions, as well as their information.")]
    List<DrumInformation> drumInformationList;
    
    [SerializeField] [Tooltip("Select the drumCorruptionPrefab object from the 'Prefabs' folder.")]
    GameObject drumCorruptionPrefab;

    List<DrumCorruption> drumCorruptions = new List<DrumCorruption>();

    OverallCorruption overallCorruption;

    float corruptionAmount;
    float distortionAmount;

    void Awake()
    {
        if(drumInformationList.Count <= 0)
            Assert.IsNotNull(drumCorruptionPrefab);
    }

    void Start () {

        overallCorruption = GetComponent<OverallCorruption>();
        Assert.IsNotNull(overallCorruption, "Please make sure that the DrumCorruptionHandler script is on the same object as the OverallCorruption script.");

		foreach(DrumInformation drumInformation in drumInformationList)
        {
            GameObject go = Instantiate(drumCorruptionPrefab, gameObject.transform);
            DrumCorruption drumCorruption = go.GetComponent<DrumCorruption>();
            drumCorruptions.Add(drumCorruption);
            drumCorruption.beats = drumInformation.beats;
            drumCorruption.perfectRange = drumInformation.perfectRange;
            drumCorruption.okayRange = drumInformation.okayRange;
            drumCorruption.duration = drumInformation.duration;
            drumCorruption.maxDistortion = drumInformation.maxDistortion;
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

    public override float GetDistortionAmount()
    {
        return distortionAmount;
    }

    public override float GetCorruptionAmount()
    {
        return corruptionAmount;
    }
}
