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

    float overallCorruption;
    float overallDistortion;

    List<CorruptionHandlerBaseClass> corruptionHandlers;
    AudioDistortion audioDistortion;
    List<CorruptionBaseClass> corruptions = new List<CorruptionBaseClass>();

    void Start () {
        corruptionHandlers = new List<CorruptionHandlerBaseClass>(GetComponents<CorruptionHandlerBaseClass>());
        audioDistortion = GameManager.Instance.audioDistortion;
        foreach (CorruptionHandlerBaseClass corruptionHandler in corruptionHandlers)
        {
            corruptions.AddRange(corruptionHandler.corruptions);
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
}
