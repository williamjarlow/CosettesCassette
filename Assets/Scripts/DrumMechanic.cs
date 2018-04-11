using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This script may be transported to AudioTimeline

public class DrumMechanic : MonoBehaviour {

    /*[HideInInspector]*/ public List<int> inputTimeStamps;

    private AudioManager audioManager;
    private AudioSource audioSource;
    [SerializeField] private int timeStamp;
    private bool recording = false;
    private bool isPlaying;
    [SerializeField] private int iterator = 0;
    [SerializeField] private string bassDrumPath;


    void Start ()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        audioSource = GetComponent<AudioSource>();

        //Set audio buffer size to reduce delay
        AudioSettings.SetDSPBufferSize(256, 2);

        Debug.Assert(audioManager != null, "Could not find the Audio Manager");
	}


    void FixedUpdate()
    {
        //Find the timeline position in the track
        audioManager.gameMusicEv.getTimelinePosition(out timeStamp);

        if (recording)
        {
            // If we have started the track and there is some form of input
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0) && timeStamp > 0 && !isPlaying)
            {
                //Play the bass drum sound and record it by adding the current time stamp to the list.
                //FMODUnity.RuntimeManager.PlayOneShot(bassDrumPath);

                isPlaying = true;
                audioSource.Play();
                inputTimeStamps.Add(timeStamp);
                Debug.LogError("Beat fixed: " + Time.time.ToString());

                StartCoroutine(ResetPlayed());
            }
        }


        else if (recording == false)
        {
            //  Make sure the list is not empty and check if timeStamp matches the value in the list[i]. Also check if we are at the last index in the list
            // When the item in the list matches the current timestamp, play the bass drum sound (play the recorded audio)
            if (inputTimeStamps.Count > 0 && timeStamp == inputTimeStamps[iterator] && iterator < inputTimeStamps.Count - 1)
            {
                //FMODUnity.RuntimeManager.PlayOneShot(bassDrumPath);
                audioSource.Play();
                iterator++;
            }
        }


    }


    private IEnumerator ResetPlayed()
    {
        yield return new WaitForSeconds(0.05f);
        isPlaying = false;
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
