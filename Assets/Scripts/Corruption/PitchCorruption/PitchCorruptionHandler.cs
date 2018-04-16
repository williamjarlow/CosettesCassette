using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class PitchInformation
{
    public Duration duration;
    [Range(0, 100)]
    public float maxDistortion;
    [HideInInspector]
    public float currentDistortion;
}

public class PitchCorruptionHandler : CorruptionHandlerBaseClass
{
    [SerializeField] [Tooltip("Amount of pitch corruptions, as well as their information.")]
    List<PitchInformation> pitchInformationList;
    
    [SerializeField] [Tooltip("Select the pitchCorruptionPrefab object from the 'Prefabs' folder.")]
    GameObject pitchCorruptionPrefab;

    List<PitchCorruption> pitchCorruptions = new List<PitchCorruption>();

    OverallCorruption overallCorruption;

    void Awake()
    {
        if (pitchInformationList.Count <= 0)
            Assert.IsNotNull(pitchCorruptionPrefab);
    }

    void Start()
    {

        overallCorruption = GetComponent<OverallCorruption>();
        Assert.IsNotNull(overallCorruption, "Please make sure that the DrumCorruptionHandler script is on the same object as the OverallCorruption script.");

        foreach (PitchInformation pitchInformation in pitchInformationList)
        {
            GameObject go = Instantiate(pitchCorruptionPrefab, gameObject.transform);
            PitchCorruption pitchCorruption = go.GetComponent<PitchCorruption>();
            pitchCorruptions.Add(pitchCorruption);
            pitchCorruption.duration = pitchInformation.duration;
            pitchCorruption.maxDistortion = pitchInformation.maxDistortion;
        }
    }

    void Update()
    {

    }

    public override void UpdateDistortionAmount()
    {
        distortionAmount = 0;
        foreach (PitchCorruption pitchCorruption in pitchCorruptions)
        {
            distortionAmount += pitchCorruption.innerDistortion;
        }
        overallCorruption.UpdateDistortionAmount();
    }

    public override void UpdateCorruptionAmount()
    {
        corruptionAmount = 0;
        foreach (PitchCorruption pitchCorruption in pitchCorruptions)
        {
            corruptionAmount += 100 - pitchCorruption.corruptionClearedPercent / pitchCorruptions.Count;
        }
        overallCorruption.UpdateCorruptionAmount();
    }
}
