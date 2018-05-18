using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// ** Written by Hannes Gustafsson ** //


public class DrumMechanic : MonoBehaviour {

    public List<int> inputTimeStamps;

    private AudioManager audioManager;
    private FMOD.RESULT result;

    GameManager gameManager;

    private FMOD.Sound kickSound;
    private FMOD.Sound kickSubSound;
    private FMOD.Studio.SOUND_INFO kickInfo;
    private FMOD.ChannelGroup kickChannelGroup;
    private FMOD.Channel kickChannel;

    private bool isPlaying;
    [HideInInspector] public bool gaveInput = false;

    [SerializeField] private int timeStamp;
    [Tooltip("Time tolerance in ms when comparing timeline position and recorded beats")][SerializeField] private int tolerance;

    private OverallCorruption overallCorruption;
    private ButtonDisabler buttonDisabler;

    //** TODO ** //
    // 1. Fix recording not stopping when you rewind to before the segment
    // 2. The audio input when recording is playing more than once

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Start ()
    {
        audioManager = gameManager.audioManager;
        overallCorruption = gameManager.overallCorruption;
        buttonDisabler = gameManager.uiHandler.GetComponent<ButtonDisabler>();

  //      Debug.Assert(audioManager != null, "Could not find the Audio Manager");
  //      Debug.Log(" *** Create sound result *** --> " + result);

		//result = audioManager.systemObj.getSoundInfo(audioManager.bassDrumKey, out kickInfo);
		//Debug.Log("info " + result);
		//kickInfo.mode = FMOD.MODE.CREATESAMPLE;

		//result = audioManager.lowLevelSys.createSound(kickInfo.name_or_data, kickInfo.mode, ref kickInfo.exinfo, out kickSound);
		//Debug.Log(" *** Create sound result *** --> " + result);
		//result = kickSound.getSubSound(0, out kickSubSound);
		//Debug.Log("subSound: " + result);
    }


    void FixedUpdate()
    {
        //Find the timeline position in the track
        audioManager.gameMusicEv.getTimelinePosition(out timeStamp);

        if (gameManager.recording && overallCorruption.durations[gameManager.currentSegmentIndex].recordingType == Duration.RecordingType.DRUMS)
        {
            // If we have started the track and there is some form of input
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0) && !isPlaying)
            {
                //Play the bass drum sound and record it by adding the current time stamp to the list.
                //result = audioManager.lowLevelSys.playSound(kickSubSound, kickChannelGroup, false, out kickChannel);
                //Debug.Log("Kick: " + result);

                overallCorruption.durations[gameManager.currentSegmentIndex].AddDrumRecordings(timeStamp);
                isPlaying = true;
                gaveInput = true;

                StartCoroutine(ResetPlayed());
                    
            }
        }

        else if(gameManager.recording == false && !isPlaying && !audioManager.switchedToAudioLog)
        {
            // Loop through the corrupted segments
            for(int i = 0; i < overallCorruption.durations.Count; i++)
            {
                // Check if we are in a corrupted segment
                if(timeStamp >= overallCorruption.durations[i].start && timeStamp < overallCorruption.durations[i].stop)
                {
                    // Loop through the list of recorded drums at segment [i]
                    for(int j = 0; j < overallCorruption.durations[i].GetDrumRecordings().Count; j++)
                    {
                        // Play the recorded sound when the current timestamp matches the recorded timestamp
                        if(timeStamp <= overallCorruption.durations[i].GetDrumRecordings()[j] + tolerance / 2 && timeStamp >= overallCorruption.durations[i].GetDrumRecordings()[j] - tolerance / 2)
                        {
                            //result = audioManager.lowLevelSys.playSound(kickSubSound, kickChannelGroup, false, out kickChannel);
                            isPlaying = true;
                            StartCoroutine(ResetPlayed());
                        }
                    }
                }
            }
            
        }

        
    }

    private void LateUpdate()
    {
        // If we exited the corrupted segment, stop recording
        //if (timeStamp < overallCorruption.durations[currentSegmentIndex].start || timeStamp > overallCorruption.durations[currentSegmentIndex].stop)
        if (timeStamp > overallCorruption.durations[gameManager.currentSegmentIndex].stop)
        {
            gameManager.Listen();
        }
    }

    private IEnumerator ResetPlayed()
    {
        yield return new WaitForSeconds(0.05f);
        isPlaying = false;
    }

    //    **** TEMPORARY SAVINGS **** //
    /*     
    *     
    *     void FixedUpdate()
   {
       //Find the timeline position in the track
       audioManager.gameMusicEv.getTimelinePosition(out timeStamp);

       if (recording)
       {
           // If we have started the track and there is some form of input
           if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0) && !isPlaying)
           {
               //Play the bass drum sound and record it by adding the current time stamp to the list.
               // result = audioManager.lowLevelSys.playSound(kick, channelGroup, false, out channel);
               //Debug.Log(result);
               audioSource.Play();

               inputTimeStamps.Add(timeStamp);
               isPlaying = true;
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
                          // audioManager.lowLevelSys.playSound(kick, channelGroup, false, out channel);
                           audioSource.Play();
                           isPlaying = true;
                           StartCoroutine(ResetPlayed());
                       }

                   }
               }
           }
       }



   public void Record(GameObject confirmationObj)
   {

       // If there is no previous recording, there is a track running and we are at the music side of the cassette --> start recording
       if (inputTimeStamps.Count <= 0 && timeStamp > 0 && !audioManager.switchedToAudioLog)
       {
           recording = true;
       }

       // If there is a previous recording and there is a track running --> activate the confirmation window
       else if (inputTimeStamps.Count > 0 && timeStamp > 0)
       {
           confirmationObj.SetActive(true);
       }
  }


        // If 'YES' is pressed during confirmation --> Delete previous recordings, start recording and disable the confirmation window
   public void YesConfirmation(GameObject confirmationObj)
   {
       // If we are at the music cassette 
       if (!audioManager.switchedToAudioLog)
       {
           inputTimeStamps.Clear();
           confirmationObj.SetActive(false);
           buttonDisabler.DisableButtons();
           recording = true;
       }

   }

    */

}
