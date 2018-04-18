using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour {

    [SerializeField] private GameObject stopper;
    [SerializeField] private float currentPos;
    [SerializeField] private float endOfTheLine;
    [SerializeField] private GameObject AudioManagerz;
    private AudioManager audioManager;
    private float audiolength;
    private float audioPos;
    private float audioP;


    // Use this for initialization
    void Start ()
    {
        audioManager = AudioManagerz.GetComponent<AudioManager>();
        endOfTheLine = 2260.116f;
        audiolength = audioManager.GetTrackLength();
    }

    // Update is called once per frame
    void Update ()
    {

        audioPos = audioManager.GetTimeLinePosition();
        audioP = audioPos / audiolength;
        currentPos = audioP * endOfTheLine;
        Vector3 temp = transform.localPosition;
        temp.y = currentPos - 1100;
        transform.localPosition = temp;

    }
}
