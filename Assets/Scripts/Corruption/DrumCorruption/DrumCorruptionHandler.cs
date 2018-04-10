using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrumInformation{
    public List<int> beats;
    public int perfectRange;
    public int okayRange;
    public Duration duration;
}

public class DrumCorruptionHandler : CorruptionHandlerBaseClass {

    [SerializeField]
    List<DrumInformation> drumInformationList;
    [SerializeField]
    GameObject drumCorruptionPrefab;

    // Use this for initialization
    void Start () {
		foreach(DrumInformation drumInformation in drumInformationList)
        {
            GameObject go = Instantiate(drumCorruptionPrefab, gameObject.transform);
            DrumCorruption drumCorruption = go.GetComponent<DrumCorruption>();
            drumCorruption.beats = drumInformation.beats;
            drumCorruption.perfectRange = drumInformation.perfectRange;
            drumCorruption.okayRange = drumInformation.okayRange;
            drumCorruption.duration = drumInformation.duration;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
