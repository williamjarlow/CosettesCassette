using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverallCorruption : MonoBehaviour {

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
        audioDistortion.SetDistortion(overallDistortionMax);
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Overall corruption: " + overallCorruption  + "%");
        }
	}

    public void UpdateCorruptionAmount()
    {
        overallCorruption = 0;
        foreach (CorruptionHandlerBaseClass corruptionHandler in corruptionHandlers)
        {
            overallCorruption += corruptionHandler.GetCorruptionAmount() / corruptionHandlers.Count;
        }
    } 

    public void UpdateDistortionAmount()
    {
        overallDistortion = 0;
        foreach (CorruptionHandlerBaseClass corruptionHandler in corruptionHandlers)
        {
            overallDistortion += corruptionHandler.GetDistortionAmount();
        }
        overallDistortion += overallCorruption * overallDistortionMax / 100;
        audioDistortion.SetDistortion(overallDistortion);
        Debug.Log(audioDistortion.GetDistortion());
    }

    public float GetOverallCorruptionAmount()
    {
        return overallCorruption;
    }
}
