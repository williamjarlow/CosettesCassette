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
	private FMOD.Studio.EventInstance segmentClearEvent;
	private FMOD.Studio.EventInstance levelClearEvent;
	private FMOD.Studio.EventInstance playerFadeEv;

	//Event descriptions
	private FMOD.Studio.EventDescription musicEventDesc;
	private FMOD.Studio.EventDescription logEventDesc;
	private FMOD.Studio.EventDescription skipEventDesc;
	private FMOD.Studio.EventDescription segmentClearEventDesc;
	private FMOD.Studio.EventDescription levelClearEventDesc;
	private FMOD.Studio.EventDescription playerFadeEventDesc;

	//Event paths
	[FMODUnity.EventRef]
	[SerializeField] private string musicPath;
	[FMODUnity.EventRef]
	[SerializeField] private string audioLogPath;
	[FMODUnity.EventRef]
	[SerializeField] private string levelClearPath;
	[FMODUnity.EventRef]
	[SerializeField] private string segmentClearPath;

	//Audio table keys
    public string bassDrumKey;

	private int trackLength;

    [Tooltip("Bank files to load, should only be the file name in the directory, eg. 'Cassette_01'")]
    [SerializeField] private List<string> bankFiles;
	private FMOD.Studio.Bank[] banks = new FMOD.Studio.Bank[3];

    [HideInInspector] public bool switchedToAudioLog = false;
    private bool startedMusic = false;
    private bool pausedMusic = false;
	private bool haveDSP = false;

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

		//assigns DSPs if they haven't been assigned already
		if (!haveDSP) 
		{
			StartCoroutine (GetDSP ());
			haveDSP = true;
		}
    }

	public void AudioStopMusic ()
    {
        gameMusicEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		gameMusicEv.release ();

		playerFadeEv.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

	public void AudioPauseMusic ()
    {
		//play "pause" event
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
		//play "play" event
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
            pausedMusic = true;
            AudioPauseMusic();
        }

        else if (startedMusic && pausedMusic)
        {
            pausedMusic = false;
            AudioUnpauseMusic();
        }

        if (!startedMusic)
        {
            AudioPlayMusic();
            startedMusic = true;
        }
    }

	public void PlaySkip()
	{
		result = systemObj.getEvent("event:/Interface/Playback/skip", out skipEventDesc);
		skipEventDesc.createInstance(out skipEv);
		skipEv.setParameterValue("skip_click", 0);
		skipEv.start();
		print (result);
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

	public void PlaySegmentClear(float score)
	{
		systemObj.getEvent(segmentClearPath, out segmentClearEventDesc);
		segmentClearEventDesc.createInstance(out segmentClearEvent);
		segmentClearEvent.setParameterValue ("zone_clear_score", score);
		segmentClearEvent.start();
		segmentClearEvent.release ();
	}

	public void PlayWinSound(float score)
	{
		systemObj.getEvent(levelClearPath, out levelClearEventDesc);
		levelClearEventDesc.createInstance(out levelClearEvent);
		levelClearEvent.setParameterValue ("stage_clear_score", score);
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

	private IEnumerator GetDSP()
	{
		FMOD.Studio.PLAYBACK_STATE state;

		gameMusicEv.getPlaybackState (out state);

		//waits for "gameMusicEv" to start before trying to get DSPs
		while (state != FMOD.Studio.PLAYBACK_STATE.PLAYING) 
		{
			gameMusicEv.getPlaybackState (out state);
			print("waiting for music to start");
			yield return null;
		}
		
		FMOD.DSPConnection DSPCon;

		gameMusicEv.getChannelGroup (out musicChanGroup);
		musicChanGroup.getGroup (0, out musicChanSubGroup);
		musicChanSubGroup.getDSP (3, out musicChanSubGroupDSP); // 3 carries channels of instrument tracks, 0 is the sum track
		musicChanSubGroupDSP.getInput (0, out pitchChordsDSP, out DSPCon); // adding effects in studio messes up 4 and beyond
		musicChanSubGroupDSP.getInput (1, out pitchVocalsDSP, out DSPCon);
		musicChanSubGroupDSP.getInput (2, out pitchDrumsDSP, out DSPCon);
		musicChanSubGroupDSP.getInput (3, out pitchBassDSP, out DSPCon);
		musicChanSubGroupDSP.getInput (4, out pitchLeadDSP, out DSPCon);

		// *** TEMP ***
		pitchChordsDSP.setBypass(false);
		pitchVocalsDSP.setBypass(false);
		pitchDrumsDSP.setBypass(false);
		pitchBassDSP.setBypass(false);
		pitchLeadDSP.setBypass(false);

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
		//Destroy FMOD system objects (safety precaution, likely not needed anymore)
		lowLevelSys.release();
        systemObj.release();
    }


}