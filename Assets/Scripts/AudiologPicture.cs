using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class AudiologPicture : MonoBehaviour {

    [SerializeField] private Image AudiologPic;
    [HideInInspector] public float currentPos;
    private float trackLength;
    private GameObject AudioManager;
    [Tooltip("Time in milliseconds from start of audiolog to when picture appears.")]
    [SerializeField] private float pictureAppearTime;
    private AudioManager audioManager;
    private float audiolength;
    private float audiologlength;
    private float audioPos;
    [HideInInspector] public bool showpicture = false;

    // Use this for initialization
    void Start()
    {

        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        audiolength = audioManager.GetTrackLength();
        trackLength = audioManager.GetTrackLength();

        AudiologPic.CrossFadeAlpha(0, 0.0f, false);
        AudiologPic.enabled = false;

        if (showpicture == true)
        {
            AudiologPic.CrossFadeAlpha(0, 0.0f, false);
        }

        
    }

    // Update is called once per frame
    void Update() {

        if (showpicture)
        {
            if (audioManager.switchedToAudioLog)
                AudiologPic.enabled = true;
            else
                AudiologPic.enabled = false;
        }

        if (audioManager.switchedToAudioLog == true)
        {
            //transform.localPosition = new Vector3(transform.localPosition.x, audioManager.GetTimeLinePosition(), transform.localPosition.z);
            if (audioManager.GetTimeLinePosition() > pictureAppearTime && showpicture == false)
            {
                Fade();
                showpicture = true;
            }
        }

    }

    public void Fade()
    {
        AudiologPic.enabled = true;
        AudiologPic.CrossFadeAlpha(1, 1.0f, false);
    }
}