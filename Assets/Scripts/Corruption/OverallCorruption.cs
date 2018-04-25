using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionInformation {
    [Range(0, 100)]
    public float maxDistortion;
    [HideInInspector]
    public float currentDistortion;
    [Range(0, 100)]
    public int clearThreshold;
    [Header("ID of segment in 'Game Manager'")]
    public int segmentID;
}

public class OverallCorruption : MonoBehaviour {
    [SerializeField]
    [Range(0, 100)]
    float overallDistortionMax;

    // The list of beats
    public List<Duration> segments;
    [Range(1, 200)]
    public int bpm;
    const int beatsToMsConversion = 15000;

    [HideInInspector]
    public int bpmInMs;

    [Tooltip("Percent of corruption that has to be cleared before corruption is considered solved.")][SerializeField] int corruptionClearThreshold;

    [HideInInspector] public float overallCorruption;
    [HideInInspector] public float overallDistortion;

    [HideInInspector]
    // The list of beats converted to milliseconds
    public List<Duration> durations;

    List<CorruptionHandlerBaseClass> corruptionHandlers;
    AudioDistortion audioDistortion;
    List<CorruptionBaseClass> corruptions = new List<CorruptionBaseClass>();

    // ** Corrupted Area** //
    [SerializeField]
    [Tooltip("Corrupted area prefab")]
    private GameObject corruptedArea;
    private List<GameObject> corruptedAreaList = new List<GameObject>();

    void Awake () {
        corruptionHandlers = new List<CorruptionHandlerBaseClass>(GetComponents<CorruptionHandlerBaseClass>());

        bpmInMs = ConvertBpmToMs(bpm);

        for (int i = 0; i < segments.Count; i++)
        {
            Debug.Log("REEE");
            durations.Add(new Duration());
            durations[i].start = segments[i].start * bpmInMs;
            durations[i].stop = segments[i].stop * bpmInMs;
        }
    }
	
    void Start()
    {
        audioDistortion = GameManager.Instance.audioDistortion;

        foreach (CorruptionHandlerBaseClass corruptionHandler in corruptionHandlers)
        {
            corruptions.AddRange(corruptionHandler.corruptions);
        }

        Debug.Assert(corruptedArea != null, "Attach the corrupted area prefab to 'Overall Corruption'");

        for (int i = 0; i < segments.Count; i++)
        {
            // Instantiate the corrupted area prefab according to the corrupted area specifications
            RectTransform timelineSlider = GameManager.Instance.timelineSlider.GetComponent<RectTransform>();
            GameObject instantiatedObject = Instantiate(corruptedArea, timelineSlider);
            instantiatedObject.transform.SetAsFirstSibling();
            corruptedAreaList.Add(instantiatedObject);
            instantiatedObject.GetComponent<CorruptionVisuals>().SetCorruptionPosition(durations[i].start, durations[i].stop);
        }

        UpdateCorruptionAmount();
        UpdateDistortionAmount();
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
            if (corruption.corruptionClearedPercent >= corruption.clearThreshold)
                corruption.cleared = true;
            else
                corruption.cleared = false;
        }
        for (int i = 0; i < segments.Count; i++)
        {
            bool cleared = true;
            foreach (CorruptionBaseClass corruption in corruptions)
            {
                if (i == corruption.segmentID)
                {
                    if (corruption.cleared != true)
                        cleared = false;
                }
            }
            if (cleared)
            {
                corruptedAreaList[i].GetComponent<CorruptionVisuals>().RestoreOriginalColor();
            }
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
        return Mathf.RoundToInt(beatsToMsConversion / bpm);
    }

}
