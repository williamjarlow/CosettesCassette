using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverallCorruption : MonoBehaviour {

    bool lateStart = true;

    [SerializeField]
    [Range(0, 100)]
    float overallDistortionMax;

    float overallCorruption;
    float overallDistortion;

    List<CorruptionHandlerBaseClass> corruptionHandlers;
    AudioDistortion audioDistortion;

	void Start () {
        corruptionHandlers = new List<CorruptionHandlerBaseClass>(FindObjectsOfType<CorruptionHandlerBaseClass>());
        audioDistortion = GameObject.FindWithTag("AudioManager").GetComponent<AudioDistortion>();
	}
	
	void Update () {
        if (lateStart)
        {
            lateStart = false;
            audioDistortion.SetDistortion(overallDistortionMax);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Overall corruption: " + overallCorruption  + "%");
            Debug.Log("Overall distortion: " + overallDistortion + "%");
        }
	}

    public void UpdateCorruptionAmount()
    {
        overallCorruption = 0;
        foreach (CorruptionHandlerBaseClass corruptionHandler in corruptionHandlers)
        {
            overallCorruption += corruptionHandler.corruptionAmount / corruptionHandlers.Count;
        }
    } 

    public void UpdateDistortionAmount()
    {
        overallDistortion = 0;
        foreach (CorruptionHandlerBaseClass corruptionHandler in corruptionHandlers)
        {
            overallDistortion += corruptionHandler.distortionAmount;
        }
        overallDistortion += overallCorruption * overallDistortionMax / 100;
        audioDistortion.SetDistortion(overallDistortion);
    }

    public float GetOverallCorruptionAmount()
    {
        return overallCorruption;
    }
}
