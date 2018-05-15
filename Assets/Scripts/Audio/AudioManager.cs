using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AudioManager : MonoBehaviour {

	private FMOD.RESULT result;

	//System objects
	public FMOD.Studio.System systemObj;
	public FMOD.System lowLevelSys;

	//Channel groups
	public FMOD.ChannelGroup musicChanGroup;
	public FMOD.ChannelGroup musicChanSubGroup;

	//Buses
	private FMOD.Studio.Bus sfxBus;
	private FMOD.Studio.Bus interfaceBus;

	//DSPs
	public FMOD.DSP musicChanSubGroupDSP;
	public FMOD.DSP pitchChordsDSP;
	public FMOD.DSP pitchVocalsDSP;
	public FMOD.DSP pitchDrumsDSP;
	public FMOD.DSP pitchBassDSP;
	public FMOD.DSP pitchLeadDSP;

	//Event instances
	public FMOD.Studio.EventInstance gameMusicEv;
	private FMOD.Studio.EventInstance skipEv;
	private FMOD.Studio.EventInstance levelClearEvent;
	private FMOD.Studio.EventInstance playerFadeEv;

	//Event descriptions
	private FMOD.Studio.EventDescription musicEventDesc;
	private FMOD.Studio.EventDescription logEventDesc;
	private FMOD.Studio.EventDescription skipEventDesc;
	private FMOD.Studio.EventDescription levelClearEventDesc;
	private FMOD.Studio.EventDescription playerFadeEventDesc;

	//Event paths
	[FMODUnity.EventRef]
	[SerializeField] private string musicPath;
	[FMODUnity.EventRef]
	[SerializeField] private string audioLogPath;
	[FMODUnity.EventRef]
	[SerializeField] private string levelClearPath;

	//Audio table keys
    public string bassDrumKey;

	private int trackLength;

    [Tooltip("Bank files to load, should only be the file name in the directory, e.g. 'Cassette_01'. Master banks have to be loaded.")]
    [SerializeField] private List<string> bankFiles;
	private FMOD.Studio.Bank[] banks = new FMOD.Studio.Bank[3];

    [HideInInspector] public bool switchedToAudioLog = false;
    [HideInInspector] public bool startedMusic = false;
    [HideInInspector] public bool pausedMusic = true;

    void Awake ()
	{
        systemObj = FMODUnity.RuntimeManager.StudioSystem;
        lowLevelSys = FMODUnity.RuntimeManager.LowlevelSystem;

        //Loads the FMOD banks
        for (int i = 0; i < bankFiles.Count; i++)
        {
            FMODUnity.RuntimeManager.LoadBank(bankFiles[i] + ".bank", true);
			result = systemObj.getBank ("bank:/" + bankFiles [i], out banks [i]);
        }

		FMODUnity.RuntimeManager.WaitForAllLoads ();
        systemObj.flushCommands ();

        // Get the event description of music event, needed to get Track Length
        // Required to be in Awake() because a lot of game objects ask for the track length in Start()
		systemObj.getEvent(musicPath, out musicEventDesc);
        musicEventDesc.getLength(out trackLength);

		//Get the event description of audio log
		systemObj.getEvent (audioLogPath, out logEventDesc);
    }
		
    void Start()
    {
        Debug.Assert(bankFiles.Count > 0, "Enter the bank file names into the audio manager");
        Debug.Assert(this.tag == "AudioManager", "Set the tag of AudioManager to 'AudioManager'");

		//play "insertCassette" sound
		PlayInsertSound();
	}
		
    public void AudioPlayMusic ()
    {
		//"music" event is assigned to "gameMusicEv"
		if (!switchedToAudioLog)
		{
			result = musicEventDesc.createInstance (out gameMusicEv);

			//play "playerFade" event
			systemObj.getEvent ("event:/SFX/playerFade", out playerFadeEventDesc);
			playerFadeEventDesc.createInstance (out playerFadeEv);
			playerFadeEv.start ();
			playerFadeEv.release ();
		}

		//audio log event is assigned to "gameMusicEv"
        if(switchedToAudioLog)
			logEventDesc.createInstance(out gameMusicEv);

        gameMusicEv.start();

        //assigns DSPs if starting music and they haven't been assigned already
        if (!startedMusic & !switchedToAudioLog) 
			StartCoroutine (GetDSP ());
    }

	public void AudioStopMusic ()
    {
        gameMusicEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		gameMusicEv.release ();

		playerFadeEv.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        startedMusic = false;
    }

	public void AudioPauseMusic ()
    {
		FMOD.Studio.EventDescription pauseEventDesc;
	 	FMOD.Studio.EventInstance pauseEv;
		systemObj.getEvent ("event:/Interface/Playback/pause", out pauseEventDesc);
		pauseEventDesc.createInstance (out pauseEv);
		pauseEv.start ();
		pauseEv.release ();

		playerFadeEv.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        gameMusicEv.setPaused(true);
    }

	public void AudioUnpauseMusic ()
    {
		FMOD.Studio.EventDescription playEventDesc;
		FMOD.Studio.EventInstance playEv;
		systemObj.getEvent ("event:/Interface/Playback/play", out playEventDesc);
		playEventDesc.createInstance (out playEv);
		playEv.start ();
		playEv.release ();

        gameMusicEv.setPaused(false);
    }

    public void AudioPlayPauseAndUnpause()
    {
        if (startedMusic && !pausedMusic)
        {
            AudioPauseMusic();
            pausedMusic = true;
        }

        else if (startedMusic && pausedMusic)
        {
            AudioUnpauseMusic();
            pausedMusic = false;
        }

        if (!startedMusic)
        {
            AudioPlayMusic();
            startedMusic = true;
            pausedMusic = false;
        }
    }

	public void PlaySkip()
	{
		result = systemObj.getEvent("event:/Interface/Playback/skip", out skipEventDesc);
		skipEventDesc.createInstance(out skipEv);
		skipEv.setParameterValue("skip_click", 0);
		skipEv.start();
	}

	public void SetSkipPitch(float speed)
	{
		skipEv.setParameterValue("skip_pitch", speed);
	}

	public void StopPlayingSkip()
	{
		skipEv.setParameterValue("skip_click", 1);
		skipEv.release ();
	}

	public void PlayRecordStart()
	{
		FMOD.Studio.EventDescription recordStartEventDesc;
		FMOD.Studio.EventInstance recordStartEv;
		systemObj.getEvent ("event:/Interface/Playback/record_start", out recordStartEventDesc);
		recordStartEventDesc.createInstance (out recordStartEv);
		recordStartEv.start ();
		recordStartEv.release ();
	}

	public void PlayRecordStop()
	{
		FMOD.Studio.EventDescription recordStopEventDesc;
		FMOD.Studio.EventInstance recordStopEv;
		systemObj.getEvent ("event:/Interface/Playback/record_stop", out recordStopEventDesc);
		recordStopEventDesc.createInstance (out recordStopEv);
		recordStopEv.start ();
		recordStopEv.release ();
	}

	public void PlaySegmentClear(float score)
	{
		FMOD.Studio.EventDescription segmentClearEventDesc;
		FMOD.Studio.EventInstance segmentClearEvent;
		systemObj.getEvent("event:/SFX/segment_clear", out segmentClearEventDesc);
		segmentClearEventDesc.createInstance(out segmentClearEvent);
		segmentClearEvent.setParameterValue ("segment_clear_score", score); // 0 = okay, 1 = perfect
		segmentClearEvent.start();
		segmentClearEvent.release ();
	}

	public void PlayWinSound(float score)
	{
		systemObj.getEvent(levelClearPath, out levelClearEventDesc);
		levelClearEventDesc.createInstance(out levelClearEvent);
		levelClearEvent.setParameterValue ("stage_clear_score", score); // 0 = okay, 1 = perfect
		levelClearEvent.start();
		levelClearEvent.release ();
	}

	public void PlayInsertSound ()
	{
		FMOD.Studio.EventDescription insertCassetteEventDesc;
		FMOD.Studio.EventInstance insertCassetteEv;
		systemObj.getEvent ("event:/SFX/insertCassette", out insertCassetteEventDesc);
		insertCassetteEventDesc.createInstance (out insertCassetteEv);
		insertCassetteEv.start ();
		insertCassetteEv.release ();
	}

    public void PlayEjectSound()
    {
        FMOD.Studio.EventDescription ejectEventDesc;
        FMOD.Studio.EventInstance ejectEv;
        systemObj.getEvent("event:/Interface/Playback/eject", out ejectEventDesc);
        ejectEventDesc.createInstance(out ejectEv);
        ejectEv.start();
        ejectEv.release();
    }
		
    public void PlaySnapSound()
    {
        FMOD.Studio.EventDescription snapEventDesc;
        FMOD.Studio.EventInstance snapEv;
        systemObj.getEvent("event:/Interface/Playback/snap", out snapEventDesc);
        snapEventDesc.createInstance(out snapEv);
        snapEv.start();
        snapEv.release();
    }

	public void PlayPauseMenuOpen()
	{
		FMOD.Studio.EventDescription pauseMenuOpenEventDesc;
		FMOD.Studio.EventInstance pauseMenuOpenEv;
		systemObj.getEvent("event:/Interface/PauseMenu/open", out pauseMenuOpenEventDesc);
		pauseMenuOpenEventDesc.createInstance(out pauseMenuOpenEv);
		pauseMenuOpenEv.start();
		pauseMenuOpenEv.release();
	}

	public void PlayPauseMenuClose()
	{
		FMOD.Studio.EventDescription pauseMenuCloseEventDesc;
		FMOD.Studio.EventInstance pauseMenuCloseEv;
		systemObj.getEvent("event:/Interface/PauseMenu/close", out pauseMenuCloseEventDesc);
		pauseMenuCloseEventDesc.createInstance(out pauseMenuCloseEv);
		pauseMenuCloseEv.start();
		pauseMenuCloseEv.release();
	}

	public void PlayPauseMenuSelect()
	{
		FMOD.Studio.EventDescription pauseMenuSelectEventDesc;
		FMOD.Studio.EventInstance pauseMenuSelectEv;
		systemObj.getEvent("event:/Interface/PauseMenu/select", out pauseMenuSelectEventDesc);
		pauseMenuSelectEventDesc.createInstance(out pauseMenuSelectEv);
		pauseMenuSelectEv.start();
		pauseMenuSelectEv.release();
	}

	public void PlayPauseMenuOn()
	{
		FMOD.Studio.EventDescription pauseMenuOnEventDesc;
		FMOD.Studio.EventInstance pauseMenuOnEv;
		systemObj.getEvent("event:/Interface/PauseMenu/on", out pauseMenuOnEventDesc);
		pauseMenuOnEventDesc.createInstance(out pauseMenuOnEv);
		pauseMenuOnEv.start();
		pauseMenuOnEv.release();
	}

	public void PlayPauseMenuOff()
	{
		FMOD.Studio.EventDescription pauseMenuOffEventDesc;
		FMOD.Studio.EventInstance pauseMenuOffEv;
		systemObj.getEvent("event:/Interface/PauseMenu/off", out pauseMenuOffEventDesc);
		pauseMenuOffEventDesc.createInstance(out pauseMenuOffEv);
		pauseMenuOffEv.start();
		pauseMenuOffEv.release();
	}

	public void PlayShootSound(float result)
	{
		FMOD.Studio.EventDescription shootEventDesc;
		FMOD.Studio.EventInstance shootEv;
		systemObj.getEvent("event:/SFX/shoot", out shootEventDesc);
		shootEventDesc.createInstance(out shootEv);
		shootEv.setParameterValue ("shoot_result", result); //0 = hit, 1 = wrong, 2 = miss
		shootEv.start();
		shootEv.release();
	}

	public void MuteSFX(bool mute)
	{
		systemObj.getBus ("bus:/SFX", out sfxBus);
		systemObj.getBus ("bus:/Interface", out interfaceBus);

		if(!mute)
			StartCoroutine (delayMute (0.103f)); //waits until "off" event finishes
		else if (mute) 
		{
			sfxBus.setMute (false);
			interfaceBus.setMute (false);
		}
	}

	private IEnumerator delayMute(float time)
	{
		yield return new WaitForSeconds (time);

		sfxBus.setMute (true);
		interfaceBus.setMute (true);
	}
		
    private IEnumerator GetDSP()
	{
		FMOD.Studio.PLAYBACK_STATE state;

		gameMusicEv.getPlaybackState (out state);

		//waits for "gameMusicEv" to start before trying to get DSPs
		while (state != FMOD.Studio.PLAYBACK_STATE.PLAYING) 
		{
			gameMusicEv.getPlaybackState (out state);
			yield return null;
		}
		
		FMOD.DSPConnection DSPCon;
		//FMOD.DSP_TYPE type;

		gameMusicEv.getChannelGroup (out musicChanGroup);
		result = musicChanGroup.getGroup (0, out musicChanSubGroup);
		//print ("Get subgroup: " + result);
		result = musicChanSubGroup.getDSP (3, out musicChanSubGroupDSP);
		//print ("Get subgroup DSP: " + result);
		result = musicChanSubGroupDSP.getInput (0, out pitchChordsDSP, out DSPCon);
//		pitchChordsDSP.getType (out type);
//		print (result);
//		print (type);
		result = musicChanSubGroupDSP.getInput (1, out pitchVocalsDSP, out DSPCon);
		/*pitchLeadDSP.getType (out type);
		print (result);
		print (type);*/
		result = musicChanSubGroupDSP.getInput (2, out pitchDrumsDSP, out DSPCon);
		/*pitchBassDSP.getType (out type);
		print (result);
		print (type);*/
		result = musicChanSubGroupDSP.getInput (3, out pitchBassDSP, out DSPCon);
		/*pitchVocalsDSP.getType (out type);
		print (result);
		print (type);*/
		result = musicChanSubGroupDSP.getInput (4, out pitchLeadDSP, out DSPCon);
		/*pitchDrumsDSP.getType (out type);
		print (result);
		print (type);*/

		/*pitchChordsDSP.setBypass(false);
		pitchVocalsDSP.setBypass(false);
		pitchDrumsDSP.setBypass(false);
		pitchBassDSP.setBypass(false);
		pitchLeadDSP.setBypass(false);*/
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
        if (switchedToAudioLog)
            logEventDesc.getLength(out trackLength);
        if (!switchedToAudioLog)
            musicEventDesc.getLength(out trackLength);
        startedMusic = false;
    }

    private void OnDestroy()
    {
        //release FMOD system objects (safety precaution, likely not needed anymore). "systemObj" has to be released first.
        systemObj.release();
	    lowLevelSys.release();
    }
}