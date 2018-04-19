using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class PitchInformation : CorruptionInformation{

    [Header("Pitch nodes")]
    public List<PitchNode> nodes = new List<PitchNode>();

    
    public Vector2 rGoalRange = new Vector2(-2, 2);
    public Vector2 rTravelTimeRange = new Vector2(1, 3);

    Duration duration;
    [Header("ID of segment in 'Game Manager'")]
    public int segmentID;

    [Header("Random generation of pitch nodes")]
    public bool randomizeNodes;

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
            PitchCorruption pitchCorruption = go.GetComponent<PitchCorruption>();
            corruptions.Add(pitchCorruption);
            pitchCorruption.maxDistortion = pitchInformation.maxDistortion;
            pitchCorruption.duration = GameManager.Instance.durations[pitchInformation.segmentID];
            if(pitchInformation.randomizeNodes)
            {
                for (int i = 0; i < pitchInformation.nodes.Count; i++)
                {
                    pitchInformation.nodes.Add(new PitchNode());
                    pitchInformation.nodes[i].seconds = Random.Range(pitchInformation.rTravelTimeRange.x, pitchInformation.rTravelTimeRange.y);
                    pitchInformation.nodes[i].position = new Vector3(gameObject.transform.localPosition.x, Random.Range(pitchInformation.rGoalRange.x, pitchInformation.rGoalRange.y), gameObject.transform.localPosition.z);
                }
            }
        }
	}
}
