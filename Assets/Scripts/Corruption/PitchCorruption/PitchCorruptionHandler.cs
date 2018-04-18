using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class PitchInformation : CorruptionInformation{
    [Header("Beat corruption placement.")] [Tooltip("16 = 1 bar. 4 = perfectly on beat.")]
    public List<int> beats;

    [Header("Ranges in milliseconds")]
    public int perfectRange;
    public int okayRange;

    Duration duration;
    [Header("ID of segment in 'Game Manager'")]
    public int segmentID;

    
}

public class PitchCorruptionHandler : CorruptionHandlerBaseClass {

    [SerializeField] [Tooltip("Amount of drum corruptions, as well as their information.")]
    List<PitchInformation> pitchInformationList;
    
    [SerializeField] [Tooltip("Select the drumCorruptionPrefab object from the 'Prefabs' folder.")]
    GameObject pitchCorruptionPrefab;

    OverallCorruption overallCorruption;

    void Awake () {

        if (pitchInformationList.Count <= 0)
            Assert.IsNotNull(pitchCorruptionPrefab);

        overallCorruption = GetComponent<OverallCorruption>();
        Assert.IsNotNull(overallCorruption, "Please make sure that the DrumCorruptionHandler script is on the same object as the OverallCorruption script.");

		foreach(PitchInformation pitchInformation in pitchInformationList) //Set starting values for corruption
        {
            GameObject go = Instantiate(pitchCorruptionPrefab, gameObject.transform);
            DrumCorruption drumCorruption = go.GetComponent<DrumCorruption>();
            corruptions.Add(drumCorruption);
            drumCorruption.beats = pitchInformation.beats;
            drumCorruption.perfectRange = pitchInformation.perfectRange;
            drumCorruption.okayRange = pitchInformation.okayRange;
            drumCorruption.maxDistortion = pitchInformation.maxDistortion;
            for (int i = 0; i < pitchInformation.beats.Count; i++)
            {
                drumCorruption.beats[i] = pitchInformation.beats[i] * GameManager.Instance.bpmInMs;
            }
            drumCorruption.duration = GameManager.Instance.durations[pitchInformation.segmentID];
        }
	}
}
