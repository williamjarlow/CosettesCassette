using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionInformation {
    [Range(0, 100)]
    public float maxDistortion;
    [HideInInspector]
    public float currentDistortion;
}

public class OverallCorruption : MonoBehaviour {
    [SerializeField]
    [Range(0, 100)]
    float overallDistortionMax;

    public List<Duration> segments;
    [Range(1, 200)]
    public int bpm;

    [HideInInspector]
    public int bpmInMs;

    float overallCorruption;
    float overallDistortion;

    [HideInInspector]
    public List<Duration> durations;

    List<CorruptionHandlerBaseClass> corruptionHandlers;
    AudioDistortion audioDistortion;
    List<CorruptionBaseClass> corruptions = new List<CorruptionBaseClass>();

    // ** TEST ** //
    [SerializeField]
    [Tooltip("Corrupted area prefab")]
    private GameObject corruptedArea;

    void Awake () {
        corruptionHandlers = new List<CorruptionHandlerBaseClass>(GetComponents<CorruptionHandlerBaseClass>());
        audioDistortion = GameManager.Instance.audioDistortion;

        bpmInMs = ConvertBpmToMs(bpm);

        for (int i = 0; i < segments.Count; i++)
        {
            durations.Add(new Duration());
            durations[i].start = segments[i].start * bpmInMs;
            durations[i].stop = segments[i].stop * bpmInMs;
        }
    }
	
    void Start()
    {

        foreach (CorruptionHandlerBaseClass corruptionHandler in corruptionHandlers)
        {
            corruptions.AddRange(corruptionHandler.corruptions);
        }
       
        UpdateCorruptionAmount();
        UpdateDistortionAmount();

        Debug.Assert(corruptedArea != null, "Attach the corrupted area prefab to 'Overall Corruption'");

        for (int i = 0; i < segments.Count; i++)
        {
            // Instantiate the corrupted area prefab according to the corrupted area specifications
            RectTransform timelineSlider = GameManager.Instance.timelineSlider.GetComponent<RectTransform>();
            GameObject instantiatedObject = Instantiate(corruptedArea, timelineSlider);
            instantiatedObject.transform.SetAsFirstSibling();
            instantiatedObject.GetComponent<CorruptionVisuals>().SetCorruptionPosition(durations[i].start, durations[i].stop);
        }
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Overall corruption: " + overallCorruption  + "%");
            Debug.Log("Overall distortion: " + overallDistortion + "%");
        }
	}

    public void UpdateCorruptionAmount()
    {
        overallCorruption = 0;
        foreach (CorruptionBaseClass corruption in corruptions)
        {
           
            overallCorruption += (100 - corruption.corruptionClearedPercent) / corruptions.Count;
        }
    } 

    public void UpdateDistortionAmount()
    {
        overallDistortion = 0;
        foreach (CorruptionBaseClass corruption in corruptions)
        {
            overallDistortion += corruption.innerDistortion; 
        }
        overallDistortion += overallCorruption * overallDistortionMax / 100;
        audioDistortion.SetDistortion(overallDistortion);
    }

    public float GetOverallCorruptionAmount()
    {
        return overallCorruption;
    }

    int ConvertBpmToMs(int bpm)
    {
        return Mathf.RoundToInt((60000 / 4) / bpm);
    }

}
