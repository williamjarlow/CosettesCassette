﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AudioManager : MonoBehaviour {

	private FMOD.RESULT result;
	public FMOD.Studio.System systemObj;
	public FMOD.System lowLevelSys;

	[FMODUnity.EventRef]
    public FMOD.Studio.EventInstance gameMusicEv;
	private FMOD.Studio.EventDescription musicEventDesc;
	private FMOD.Studio.EventDescription logEventDesc;

	private AudioDistortion audioDistortion;
	private DrumMechanic drumMechanic;

    private int trackLength;

    [SerializeField] private string musicPath;
    [SerializeField] private string audioLogPath;
    public string bassDrumKey;

    [Tooltip("Bank files to load, should only be the file name in the directory, eg. 'Cassette_01.bank'")]
    [SerializeField] private List<string> bankFiles;
	private FMOD.Studio.Bank[] banks = new FMOD.Studio.Bank[3];

    [HideInInspector] public bool switchedToAudioLog = false;
    private bool startedMusic = false;
    private bool pausedMusic = false;


    void Awake ()
	{
		FMOD.Studio.System.create(out systemObj);
		systemObj.getLowLevelSystem (out lowLevelSys);

		//FMOD system object initialization
		lowLevelSys.setSoftwareFormat(44100, FMOD.SPEAKERMODE.DEFAULT, 0);
		lowLevelSys.setDSPBufferSize(512, 4);
		lowLevelSys.setOutput(FMOD.OUTPUTTYPE.AUTODETECT);
		result = systemObj.initialize(64, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, (IntPtr)null);

		FMOD.Studio.LOADING_STATE state;


        //Loads the FMOD banks for the Unity Editor
        for (int i = 0; i < bankFiles.Count; i++)
        {
			result = systemObj.loadBankFile (Application.dataPath + "/StreamingAssets/" + bankFiles[i], FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out banks[i]);
			Debug.Log ("load bank: " + result);
            //FMODUnity.RuntimeManager.LoadBank(bankFiles[i], true);
			banks[i].getLoadingState(out state);
			Debug.Log("load state: " + state);
        }

        systemObj.flushCommands ();
		systemObj.flushSampleLoading ();

        // Get the event description of music event, needed to get Track Length
        // Required to be in Awake() because a lot of game objects ask for the track length in Start()
		systemObj.getEvent(musicPath, out musicEventDesc);
        musicEventDesc.getLength(out trackLength);
		systemObj.getEvent (audioLogPath, out logEventDesc);
    }



    void Start()
    {
        Debug.Assert(bankFiles.Count > 0, "Enter the bank file names into the audio manager");
        Debug.Assert(this.tag == "AudioManager", "Set the tag of AudioManager to 'AudioManager'");

        audioDistortion = GetComponent<AudioDistortion>();
		drumMechanic = FindObjectOfType<DrumMechanic>();
	}

	void Update(){
		systemObj.update ();
		lowLevelSys.update ();
	}


    public void AudioPlayMusic ()
    {
        if(!switchedToAudioLog)
			result = musicEventDesc.createInstance(out gameMusicEv);
		Debug.Log ("create instance " + result); 

        if(switchedToAudioLog)
			result = logEventDesc.createInstance(out gameMusicEv);
		Debug.Log ("create instance " + result); 
		
        result = gameMusicEv.start();
		Debug.Log ("start instance " + result);

		drumMechanic.LoadKick ();
		StartCoroutine (GetDSP());

		FMOD.Studio.PLAYBACK_STATE state;
		result = gameMusicEv.getPlaybackState (out state);
		Debug.Log ("playback state: " + state);
    }

	public void AudioStopMusic ()
    {
        result = gameMusicEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		Debug.Log ("stop: " + result);

		FMOD.Studio.PLAYBACK_STATE state;
		result = gameMusicEv.getPlaybackState (out state);
		Debug.Log ("playback state: " + state);
    }

	public void AudioPauseMusic ()
    {
		FMOD.Studio.EventDescription pauseEventDesc;
		FMOD.Studio.EventInstance pauseEv;
		systemObj.getEvent ("event:/Interface/Playback/pause", out pauseEventDesc);
		pauseEventDesc.createInstance (out pauseEv);
		pauseEv.start ();
        gameMusicEv.setPaused(true);
    }

	public void AudioUnpauseMusic ()
    {
		FMOD.Studio.EventDescription playEventDesc;
		FMOD.Studio.EventInstance playEv;
		systemObj.getEvent ("event:/Interface/Playback/play", out playEventDesc);
		playEventDesc.createInstance (out playEv);
		playEv.start ();
        gameMusicEv.setPaused(false);
    }

    public void AudioPlayPauseAndUnpause()
    {
        if (startedMusic && !pausedMusic)
        {
            print("Hello?");
            pausedMusic = true;
            AudioPauseMusic();

        }

        else if (startedMusic && pausedMusic)
        {
            print("Hello2?");
            pausedMusic = false;
            AudioUnpauseMusic();

        }

        if (!startedMusic)
        {
            AudioPlayMusic();
            startedMusic = true;
        }
    }

	private IEnumerator GetDSP()
	{
		FMOD.ChannelGroup musicChanGroup;
		FMOD.ChannelGroup musicChanSubGroup;

		FMOD.DSPConnection DSPCon;

		FMOD.DSP musicChanSubGroupDSP;
		FMOD.DSP pitchChordsDSP;
		FMOD.DSP pitchVocalsDSP;
		FMOD.DSP pitchDrumsDSP;
		FMOD.DSP pitchBassDSP;
		FMOD.DSP pitchLeadDSP;

		yield return new WaitForSeconds(0.5f); // Gives event time to load (better solution needed)
		result = gameMusicEv.getChannelGroup (out musicChanGroup);

		result = musicChanGroup.getGroup (0, out musicChanSubGroup);
		result = musicChanSubGroup.getDSP (3, out musicChanSubGroupDSP); // 3 carries channels of instrument tracks, 0 is the sum track
		result = musicChanSubGroupDSP.getInput (0, out pitchChordsDSP, out DSPCon); // adding effects in studio messes up 4 and beyond
		result = musicChanSubGroupDSP.getInput (1, out pitchVocalsDSP, out DSPCon);
		result = musicChanSubGroupDSP.getInput (2, out pitchDrumsDSP, out DSPCon);
		result = musicChanSubGroupDSP.getInput (3, out pitchBassDSP, out DSPCon);
		result = musicChanSubGroupDSP.getInput (4, out pitchLeadDSP, out DSPCon);

		//in DSP 3 of subgroup
		//0 = chords
		//1 = vocals
		//2 = drums
		//3 = bass
		//4 = lead
	}

    public float GetTrackLength()
    {
        return trackLength;
    }

    public float GetTimeLinePosition()
    {
        int temp;
        gameMusicEv.getTimelinePosition(out temp);
        return temp;
    }

    public string GetMusicPath()
    {
        return musicPath;
    }

    public string GetAudioLogPath()
    {
        return audioLogPath;
    }

    public void toggleTapeSide()
    {
        switchedToAudioLog = !switchedToAudioLog;
    }

    private void OnDestroy()
    {
        //Destroy FMOD system objects because they are created each time in 'play' mode, and the unity editor does not destroy them for you when exiting 'play' mode
        systemObj.release();
        lowLevelSys.release();
    }


}