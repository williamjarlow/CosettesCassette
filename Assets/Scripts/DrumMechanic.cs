using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// ** Written by Hannes Gustafsson ** //
// ** TODO: Reset the iterator when returning to the start of the recorded segment ** //

public class DrumMechanic : MonoBehaviour {

    /*[HideInInspector]*/ public List<int> inputTimeStamps;

    private AudioManager audioManager;
    private AudioSource audioSource;
    
    [HideInInspector] public bool recording = false;
    private bool isPlaying;
    [HideInInspector] public bool gaveInput = false;

    [SerializeField] private int timeStamp;
    //[SerializeField] private int iterator = 0;
    [Tooltip("Time tolerance in ms when comparing timeline position and recorded beats")][SerializeField] private int tolerance;
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
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0) && !isPlaying)
            {
                //Play the bass drum sound and record it by adding the current time stamp to the list.
                //FMODUnity.RuntimeManager.PlayOneShot(bassDrumPath);

                isPlaying = true;
                audioSource.Play();
                inputTimeStamps.Add(timeStamp);

                gaveInput = true;

                StartCoroutine(ResetPlayed());
            }
        }

        else if(recording == false && !isPlaying)
        {
            // Check if there is something in the list and that we are not at the last item
            if (inputTimeStamps.Count > 0)
            {
                // Check if the current position in the song is between the first and last recorded input (performance)
                if(timeStamp >= inputTimeStamps[0] - tolerance / 2 && timeStamp <= inputTimeStamps[inputTimeStamps.Count - 1] + tolerance / 2)
                {
                    // Loop through the list and match the current time stamp with the recorded input. If they match --> play recorded sound (drum sound)
                    for(int i = 0; i < inputTimeStamps.Count; i++)
                    {
                        if (timeStamp < inputTimeStamps[i] + tolerance / 2 && timeStamp > inputTimeStamps[i] - tolerance / 2)
                        {
                            audioSource.Play();
                            Debug.Log("Played drum sound at index " + i);
                            isPlaying = true;
                            StartCoroutine(ResetPlayed());
                        }
                            
                    }
                }
            }
        }

        /*
        else if (recording == false)
        {
            //  Make sure the list is not empty and check if timeStamp matches the value in the list[i]. Also check if we are at the last index in the list
            // When the item in the list matches the current timestamp, play the bass drum sound (play the recorded audio)
            if (inputTimeStamps.Count > 0  && iterator < inputTimeStamps.Count - 1 && timeStamp < inputTimeStamps[iterator] + tolerance / 2 && timeStamp > inputTimeStamps[iterator] - tolerance / 2)
            {
                //FMODUnity.RuntimeManager.PlayOneShot(bassDrumPath);
                audioSource.Play();
                iterator++;
            }
        }*/




    }

    private IEnumerator ResetPlayed()
    {
        yield return new WaitForSeconds(0.05f);
        isPlaying = false;
    }
    public void Record(GameObject confirmationObj)
    {
        // If there is no previous recording and there is a track running --> start recording
        if (inputTimeStamps.Count <= 0 && timeStamp > 0)
        {
            recording = true;
        }

        // If there is a previous recording and there is a track running --> activate the confirmation window
        else if(inputTimeStamps.Count > 0 && timeStamp > 0)
        {
            confirmationObj.SetActive(true);
        }
    }

    public void Listen()
    {
        recording = false;
    }


    // If 'YES' is pressed during confirmation --> Delete previous recordings, start recording and disable the confirmation window
    public void YesConfirmation(GameObject confirmationObj)
    {
        inputTimeStamps.Clear();
        recording = true;
        confirmationObj.SetActive(false);
    }

    // If 'NO' is pressed during confirmation --> disable the confirmation window
    public void NoConfirmation(GameObject confirmationObj)
    {
        confirmationObj.SetActive(false);
    }


}
