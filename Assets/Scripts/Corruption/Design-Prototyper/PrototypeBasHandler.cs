using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasInformation
{
    [Header("Pitch Settings")]
    public int StartingPitch;
    [Range(1, 10)] public int MaxMinPitch; 
    [Header("ID of segment in 'Game Manager'")]
    public int segmentID;
}
public class PrototypeBasHandler : MonoBehaviour {

    [SerializeField] [Tooltip("Amount of Bas corruptions, as well as their information.")]
    List<BasInformation> BasList;

    [SerializeField] GameObject basCorruptionPrefab;

    OverallCorruption overallCorruption;
    // Use this for initialization
    void Start () {
        overallCorruption = GetComponent<OverallCorruption>();


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
