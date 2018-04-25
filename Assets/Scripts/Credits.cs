using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class Credits : MonoBehaviour {

    [SerializeField] private GameObject stopper;
    [SerializeField] private float origPos;
    [SerializeField] public float currentPos;
    [SerializeField] private float endOfTheLine;
    [SerializeField] private GameObject AudioManagerz;
    [SerializeField] private GameObject credits;
    private AudioManager audioManager;
    private float audiolength;
    private float audioPos;
    private float audioP;


    // Use this for initialization
    void Start ()
    {
        
        audioManager = AudioManagerz.GetComponent<AudioManager>();
        //origPos = 2200f;
        //endOfTheLine = 4400f;
        audiolength = audioManager.GetTrackLength();
        
    }

    // Update is called once per frame
    void Update ()
    {
        audioPos = audioManager.GetTimeLinePosition();
        audioP = audioPos / audiolength;
        currentPos = audioP * endOfTheLine;
        Vector3 temp = transform.localPosition;
        temp.y = currentPos - origPos;
        transform.localPosition = temp;

    }
  
}

