using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This script may be transported to AudioTimeline

public class DrumMechanic : MonoBehaviour {

    /*[HideInInspector]*/ public List<int> inputTimeStamps;

    private AudioManager audioManager;
    private int timeStamp;
    private bool recording = false;
    [SerializeField] private int iterator = 0;
    [SerializeField] private string bassDrumPath;


    void Start ()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        Debug.Assert(audioManager != null, "Could not find the Audio Manager");
	}


    void Update()
    {
        //Find the timeline position in the track
        audioManager.gameMusicEv.getTimelinePosition(out timeStamp);

        if (recording)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && timeStamp > 0 || Input.GetMouseButtonDown(0))
            {
                FMODUnity.RuntimeManager.PlayOneShot(bassDrumPath);
                inputTimeStamps.Add(timeStamp);
                Debug.LogError("beat");
            }
        }


        else if (recording == false)
        {
            //  Make sure the list is not empty and check if timeStamp matches the value in the list[i]. Also check if we are at the last index in the list
            if (inputTimeStamps.Count > 0 && timeStamp == inputTimeStamps[iterator] && iterator < inputTimeStamps.Count - 1)
            {
                FMODUnity.RuntimeManager.PlayOneShot(bassDrumPath);
                iterator++;
            }
        }


    }
        public void Record()
    {
        //  TODO: Prompt user that the previous recording (if one exists) will be deleted
        //  Delete previous recording if user confirms, do nothing if not

        inputTimeStamps.Clear();
        iterator = 0;

        recording = true;
    }

    public void Listen()
    {
        recording = false;
    }
}
