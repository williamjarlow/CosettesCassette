using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

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
	//	public FMOD.DSP musicChanSubGroupDSP;
	//	public FMOD.DSP pitchSumDSP;
	//	public FMOD.DSP pitchChordsDSP;
	//	public FMOD.DSP pitchVocalsDSP;
	//	public FMOD.DSP pitchDrumsDSP;
	//	public FMOD.DSP pitchBassDSP;
	//	public FMOD.DSP pitchLeadDSP;
	//	public FMOD.DSP tremoloVocalsDSP;
	//	public FMOD.DSP flangerVocalsDSP;

	//Parameter instances
	public FMOD.Studio.ParameterInstance togglePitchVocals;
	public FMOD.Studio.ParameterInstance togglePitchChords;
	public FMOD.Studio.ParameterInstance togglePitchDrums;
	public FMOD.Studio.ParameterInstance togglePitchBass;
	public FMOD.Studio.ParameterInstance togglePitchLead;
	public FMOD.Studio.ParameterInstance pitchVocals;
	public FMOD.Studio.ParameterInstance pitchChords;
	public FMOD.Studio.ParameterInstance pitchDrums;
	public FMOD.Studio.ParameterInstance pitchBass;
	public FMOD.Studio.ParameterInstance pitchLead;
	public FMOD.Studio.ParameterInstance oooVocals;

	//Event instances
	public FMOD.Studio.EventInstance gameMusicEv;
	private FMOD.Studio.EventInstance skipEv;
	private FMOD.Studio.EventInstance playerFadeEv;
	public FMOD.Studio.EventInstance windEv;

	//Event descriptions
	private FMOD.Studio.EventDescription musicEventDesc;
	private FMOD.Studio.EventDescription logEventDesc;
	private FMOD.Studio.EventDescription skipEventDesc;
	private FMOD.Studio.EventDescription playerFadeEventDesc;
	private FMOD.Studio.EventDescription windEventDesc;

	//Event paths
	[FMODUnity.EventRef]
	[SerializeField] private string musicPath;
	[FMODUnity.EventRef]
	[SerializeField] private string audioLogPath;
	[FMODUnity.EventRef]
	[SerializeField] private string levelClearPath;

	private int trackLength;

	[HideInInspector] public bool switchedToAudioLog = false;
	[HideInInspector] public bool startedMusic = false;
	[HideInInspector] public bool pausedMusic = true;
    [HideInInspector] public bool bSideSpecialCase = false;

	// Added for special case of LiveTutorial popup
	private LiveTutorial liveTutorial;
	private bool showedSpecialCase = false;

	private GameManager gameManager;

	void Awake ()
	{
		systemObj = FMODUnity.RuntimeManager.StudioSystem;
		lowLevelSys = FMODUnity.RuntimeManager.LowlevelSystem;

		// Get the event description of music event, needed to get Track Length
		// Required to be in Awake() because a lot of game objects ask for the track length in Start()
		systemObj.getEvent(musicPath, out musicEventDesc);
		musicEventDesc.getLength(out trackLength);

		//Get the event description of audio log
		systemObj.getEvent (audioLogPath, out logEventDesc);
	}

	void Start()
	{
		//// Special case for LiveTutorial
		if (SceneManager.GetActiveScene().name == "Cassette00")
		{
			liveTutorial = GameObject.FindGameObjectWithTag("LiveTutorial").GetComponent<LiveTutorial>();
		}
		////
		if(SceneManager.GetActiveScene().buildIndex >= 2)
			gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

		// To make sure we get audiolog for audiolog only scenes...
		if (SceneManager.GetActiveScene().buildIndex > 15 && SceneManager.GetActiveScene().buildIndex < 23)
		{
			switchedToAudioLog = true;
            bSideSpecialCase = true;
        }

		Debug.Assert(this.tag == "AudioManager", "Set the tag of AudioManager to 'AudioManager'");
	}

	void Update()
	{
		//// Special case for LiveTutorial
		if (liveTutorial != null && switchedToAudioLog && !showedSpecialCase)
		{
			if (GetTimeLinePosition() >= GetTrackLength() - 300)
			{
				showedSpecialCase = true;
				liveTutorial.ForceOpenLiveTutorial("Press the Gear Icon to find more help in the future!");
			}
		}

	}
		
	public void AudioPlayMusic ()
	{
		//"music" event is assigned to "gameMusicEv"
		if (!switchedToAudioLog)
		{
			musicEventDesc.createInstance (out gameMusicEv);

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

		//assigns parameters of effects used by music event
		GetDSPParameters ();

		//assigns DSPs if starting music and they haven't been assigned already
		//if (!startedMusic & !switchedToAudioLog) 
		//	StartCoroutine (GetDSP ());
	}

	public void AudioPlayMenuMusic()
	{
		musicEventDesc.createInstance(out gameMusicEv);
		gameMusicEv.start();
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

		pausedMusic = true;
		StartCoroutine (WaitForPause ());

		playerFadeEv.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	private IEnumerator WaitForPause()
	{
		yield return new WaitForSeconds(0.140f);
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
		pausedMusic = false;
	}

	public void AudioPlayPauseAndUnpause()
	{
		if (startedMusic && !pausedMusic)
		{
			AudioPauseMusic();
		}

		else if (startedMusic && pausedMusic)
		{
			AudioUnpauseMusic();
		}

		if (!startedMusic)
		{
			AudioPlayMusic();
			startedMusic = true;
			pausedMusic = false;
		}
        gameManager.overallCorruption.UpdateCorruptionAmount();
        gameManager.overallCorruption.UpdateDistortionAmount();
    }

	#region soundEffects
	public void PlaySkip()
	{
		if(startedMusic)
		{
			result = systemObj.getEvent("event:/Interface/Playback/skip", out skipEventDesc);
			skipEventDesc.createInstance(out skipEv);
			skipEv.setParameterValue("skip_click", 0);
			skipEv.start();
		}
	}

	public void StopPlayingSkip()
	{
		if(startedMusic)
		{
			skipEv.setParameterValue("skip_click", 1);
			skipEv.release();
		}
	}

	public void SetSkipPitch(float speed)
	{
		skipEv.setParameterValue("skip_pitch", speed);
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
		segmentClearEvent.setParameterValue ("segment_clear_score", score); // 0 = okay, 1 = perfect, 2 = fail
		segmentClearEvent.start();
		segmentClearEvent.release ();
	}

	public void PlayWinSound(float score)
	{
		FMOD.Studio.EventDescription levelClearEventDesc;
		FMOD.Studio.EventInstance levelClearEvent;
		systemObj.getEvent(levelClearPath, out levelClearEventDesc);
		levelClearEventDesc.createInstance(out levelClearEvent);
		levelClearEvent.setParameterValue ("stage_clear_score", score); // 0 = okay, 1 = perfect
		levelClearEvent.start();
		levelClearEvent.release ();
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

	public void PlayPauseMenuBack()
	{
		FMOD.Studio.EventDescription pauseMenuBackEventDesc;
		FMOD.Studio.EventInstance pauseMenuBackEv;
		systemObj.getEvent("event:/Interface/PauseMenu/back", out pauseMenuBackEventDesc);
		pauseMenuBackEventDesc.createInstance(out pauseMenuBackEv);
		pauseMenuBackEv.start();
		pauseMenuBackEv.release();
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

	public void PlayPauseMenuExit()
	{
		FMOD.Studio.EventDescription pauseMenuExitEventDesc;
		FMOD.Studio.EventInstance pauseMenuExitEv;
		systemObj.getEvent("event:/Interface/PauseMenu/exit", out pauseMenuExitEventDesc);
		pauseMenuExitEventDesc.createInstance(out pauseMenuExitEv);
		pauseMenuExitEv.start();
		pauseMenuExitEv.release();
	}

	public void PlayScriptFlip()
	{
		FMOD.Studio.EventDescription scriptFlipEventDesc;
		FMOD.Studio.EventInstance scriptFlipEv;
		systemObj.getEvent("event:/Interface/PauseMenu/scriptFlip", out scriptFlipEventDesc);
		scriptFlipEventDesc.createInstance(out scriptFlipEv);
		scriptFlipEv.start();
		scriptFlipEv.release();
	}

	public void PlayLevelSelectScroll ()
	{
		FMOD.Studio.EventDescription levelSelectScrollEventDesc;
		FMOD.Studio.EventInstance levelSelectScrollEv;
		systemObj.getEvent("event:/Interface/LevelSelect/scroll", out levelSelectScrollEventDesc);
		levelSelectScrollEventDesc.createInstance(out levelSelectScrollEv);
		levelSelectScrollEv.start();
		levelSelectScrollEv.release();
	}

	public void PlayLevelSelectSelect ()
	{
		FMOD.Studio.EventDescription levelSelectSelectEventDesc;
		FMOD.Studio.EventInstance levelSelectBackEv;
		systemObj.getEvent("event:/Interface/LevelSelect/select", out levelSelectSelectEventDesc);
		levelSelectSelectEventDesc.createInstance(out levelSelectBackEv);
		levelSelectBackEv.start();
		levelSelectBackEv.release();
	}

	public void PlayLevelSelectPlay ()
	{
		FMOD.Studio.EventDescription levelSelectPlayEventDesc;
		FMOD.Studio.EventInstance levelSelectPlayEv;
		systemObj.getEvent("event:/Interface/LevelSelect/play", out levelSelectPlayEventDesc);
		levelSelectPlayEventDesc.createInstance(out levelSelectPlayEv);
		levelSelectPlayEv.start();
		levelSelectPlayEv.release();
	}

	public void PlayLevelSelectBack ()
	{
		FMOD.Studio.EventDescription levelSelectBackEventDesc;
		FMOD.Studio.EventInstance levelSelectBackEv;
		systemObj.getEvent("event:/Interface/LevelSelect/back", out levelSelectBackEventDesc);
		levelSelectBackEventDesc.createInstance(out levelSelectBackEv);
		levelSelectBackEv.start();
		levelSelectBackEv.release();
	}

	public void PlayResetOpen()
	{
		FMOD.Studio.EventDescription resetOpenEventDesc;
		FMOD.Studio.EventInstance resetOpenEv;
		systemObj.getEvent("event:/Interface/LevelSelect/reset_open", out resetOpenEventDesc);
		resetOpenEventDesc.createInstance(out resetOpenEv);
		resetOpenEv.start();
		resetOpenEv.release();
	}

	public void PlayResetYes()
	{
		FMOD.Studio.EventDescription resetYesEventDesc;
		FMOD.Studio.EventInstance resetYesEv;
		systemObj.getEvent("event:/Interface/LevelSelect/reset_yes", out resetYesEventDesc);
		resetYesEventDesc.createInstance(out resetYesEv);
		resetYesEv.start();
		resetYesEv.release();
	}

	public void PlayResetNo()
	{
		FMOD.Studio.EventDescription resetNoEventDesc;
		FMOD.Studio.EventInstance resetNoEv;
		systemObj.getEvent("event:/Interface/LevelSelect/reset_no", out resetNoEventDesc);
		resetNoEventDesc.createInstance(out resetNoEv);
		resetNoEv.start();
		resetNoEv.release();
	}

	public void PlaySkinMenuOpen()
	{
		FMOD.Studio.EventDescription skinOpenEventDesc;
		FMOD.Studio.EventInstance skinOpenEv;
		systemObj.getEvent("event:/Interface/LevelSelect/skin_open", out skinOpenEventDesc);
		skinOpenEventDesc.createInstance(out skinOpenEv);
		skinOpenEv.start();
		skinOpenEv.release();
	}

	public void PlaySkinMenuClose()
	{
		FMOD.Studio.EventDescription skinCloseEventDesc;
		FMOD.Studio.EventInstance skinCloseEv;
		systemObj.getEvent("event:/Interface/LevelSelect/skin_close", out skinCloseEventDesc);
		skinCloseEventDesc.createInstance(out skinCloseEv);
		skinCloseEv.start();
		skinCloseEv.release();
	}

	public void PlaySkinMenuSelect()
	{
		FMOD.Studio.EventDescription skinSelectEventDesc;
		FMOD.Studio.EventInstance skinSelectEv;
		systemObj.getEvent("event:/Interface/LevelSelect/skin_select", out skinSelectEventDesc);
		skinSelectEventDesc.createInstance(out skinSelectEv);
		skinSelectEv.start();
		skinSelectEv.release();
	}

	public void PlayTutorialOpen()
	{
		FMOD.Studio.EventDescription tutorialOpenEventDesc;
		FMOD.Studio.EventInstance tutorialOpenEv;
		systemObj.getEvent("event:/Interface/tutorial_open", out tutorialOpenEventDesc);
		tutorialOpenEventDesc.createInstance(out tutorialOpenEv);
		tutorialOpenEv.start();
		tutorialOpenEv.release();
	}

	public void PlayTutorialClose()
	{
		FMOD.Studio.EventDescription tutorialCloseEventDesc;
		FMOD.Studio.EventInstance tutorialCloseEv;
		systemObj.getEvent("event:/Interface/tutorial_close", out tutorialCloseEventDesc);
		tutorialCloseEventDesc.createInstance(out tutorialCloseEv);
		tutorialCloseEv.start();
		tutorialCloseEv.release();
	}

	public void PlayStickerGet ()
	{
		FMOD.Studio.EventDescription stickerGetEventDesc;
		FMOD.Studio.EventInstance stickerGetEv;
		systemObj.getEvent("event:/SFX/stickerGet", out stickerGetEventDesc);
		stickerGetEventDesc.createInstance(out stickerGetEv);
		stickerGetEv.start();
		stickerGetEv.release();
	}

	public void PlayOOOSelect ()
	{
		FMOD.Studio.EventDescription oooSelectEventDesc;
		FMOD.Studio.EventInstance oooSelectEv;
		systemObj.getEvent("event:/Interface/ooo_select", out oooSelectEventDesc);
		oooSelectEventDesc.createInstance(out oooSelectEv);
		oooSelectEv.start();
		oooSelectEv.release();
	}

	public void PlayOOOResult(float result)
	{
		FMOD.Studio.EventDescription oooResultEventDesc;
		FMOD.Studio.EventInstance oooResultEv;
		systemObj.getEvent("event:/SFX/ooo_result", out oooResultEventDesc);
		oooResultEventDesc.createInstance(out oooResultEv);
		oooResultEv.setParameterValue ("ooo_result", result);
		oooResultEv.start();
		oooResultEv.release();
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

	public void PlayRunnerJump()
	{
		FMOD.Studio.EventDescription runnerJumpEventDesc;
		FMOD.Studio.EventInstance runnerJumpEv;
		systemObj.getEvent("event:/SFX/Runner/runner_jump", out runnerJumpEventDesc);
		runnerJumpEventDesc.createInstance (out runnerJumpEv);
		runnerJumpEv.start ();
		runnerJumpEv.release ();
	}

	public void PlayRunnerLand()
	{
		FMOD.Studio.EventDescription runnerLandEventDesc;
		FMOD.Studio.EventInstance runnerLandEv;
		systemObj.getEvent("event:/SFX/Runner/runner_land", out runnerLandEventDesc);
		runnerLandEventDesc.createInstance (out runnerLandEv);
		runnerLandEv.start ();
		runnerLandEv.release ();
	}

	public void PlayRunnerHurt()
	{
		FMOD.Studio.EventDescription runnerHurtEventDesc;
		FMOD.Studio.EventInstance runnerHurtEv;
		systemObj.getEvent("event:/SFX/Runner/runner_hit", out runnerHurtEventDesc);
		runnerHurtEventDesc.createInstance (out runnerHurtEv);
		runnerHurtEv.start ();
		runnerHurtEv.release ();
	}

	public void PlayWind(float direction)
	{
		systemObj.getEvent("event:/SFX/wind", out windEventDesc);
		windEventDesc.createInstance (out windEv);
		windEv.setParameterValue ("wind_pan", direction);
		windEv.start ();
	}

	public void PlayGameStart()
	{
		FMOD.Studio.EventDescription gameStartEventDesc;
		FMOD.Studio.EventInstance gameStartEv;
		systemObj.getEvent("event:/Interface/LevelSelect/start", out gameStartEventDesc);
		gameStartEventDesc.createInstance(out gameStartEv);
		gameStartEv.start();
		gameStartEv.release();
	}

	public void PlayInsertAnimSound ()
	{
		FMOD.Studio.EventDescription insertCassetteEventDesc;
		FMOD.Studio.EventInstance insertCassetteEv;
		systemObj.getEvent ("event:/SFX/insertCassette", out insertCassetteEventDesc);
		insertCassetteEventDesc.createInstance (out insertCassetteEv);
		insertCassetteEv.start ();
		insertCassetteEv.release ();
	}

	public void PlayFlipAnimSound(bool sideIsA)
	{
		FMOD.Studio.EventDescription flipCassetteEventDesc;
		FMOD.Studio.EventInstance flipCassetteEv;

		if (!sideIsA)
			systemObj.getEvent ("event:/SFX/flipCassette_BA", out flipCassetteEventDesc);
		else
			systemObj.getEvent ("event:/SFX/flipCassette_AB", out flipCassetteEventDesc);

		flipCassetteEventDesc.createInstance (out flipCassetteEv);
		flipCassetteEv.start ();
		flipCassetteEv.release ();
	}

    public void PlayBoyfriend()
    {
        FMOD.Studio.EventDescription boyfriendEventDesc;
        FMOD.Studio.EventInstance boyfriendEv;
        systemObj.getEvent("event:/SFX/boyfriend", out boyfriendEventDesc);
        boyfriendEventDesc.createInstance(out boyfriendEv);
        boyfriendEv.start();
        boyfriendEv.release();
    }

    #endregion

    public void MuteSFX(bool mute)
	{
		systemObj.getBus ("bus:/SFX", out sfxBus);
		systemObj.getBus ("bus:/Interface", out interfaceBus);

		if(mute)
			StartCoroutine (delayMute (0.103f)); //waits until "off" event finishes
		else if (!mute) 
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

	public void GetDSPParameters()
	{
		//Toggles
		gameMusicEv.getParameter("toggle_pitch_vocals", out togglePitchVocals);
		gameMusicEv.getParameter("toggle_pitch_chords", out togglePitchChords);
		gameMusicEv.getParameter("toggle_pitch_drums", out togglePitchDrums);
		gameMusicEv.getParameter("toggle_pitch_bass", out togglePitchBass);
		gameMusicEv.getParameter("toggle_pitch_lead", out togglePitchLead);

		//Intensity
		gameMusicEv.getParameter("pitch_vocals", out pitchVocals);
		gameMusicEv.getParameter("pitch_chords", out pitchChords);
		gameMusicEv.getParameter("pitch_drums", out pitchDrums);
		gameMusicEv.getParameter("pitch_bass", out pitchBass);
		gameMusicEv.getParameter("pitch_lead", out pitchLead);
		gameMusicEv.getParameter("ooo_vocals", out oooVocals);
	}

	//    private IEnumerator GetDSP()
	//	{
	//		FMOD.Studio.PLAYBACK_STATE state;
	//
	//		gameMusicEv.getPlaybackState (out state);
	//
	//		//waits for "gameMusicEv" to start before trying to get DSPs
	//		while (state != FMOD.Studio.PLAYBACK_STATE.PLAYING) 
	//		{
	//			gameMusicEv.getPlaybackState (out state);
	//			yield return null;
	//		}
	//		
	//		FMOD.DSPConnection DSPCon;
	//		FMOD.DSP_TYPE type;
	//
	//		//CHANNEL GROUP AND DSP HEAD
	//		gameMusicEv.getChannelGroup (out musicChanGroup);
	//		result = musicChanGroup.getGroup (0, out musicChanSubGroup);
	//		//print ("Get subgroup: " + result);
	//		result = musicChanSubGroup.getDSP (3, out musicChanSubGroupDSP);
	//		//print ("Get subgroup DSP: " + result);
	//
	//		musicChanSubGroupDSP.get
	//
	//		result = musicChanSubGroupDSP.getInput (0, out pitchChordsDSP, out DSPCon);
	//		pitchChordsDSP.getType (out type);
	//		print (result);
	//		print (type);
	//		result = musicChanSubGroupDSP.getInput (1, out tremoloVocalsDSP, out DSPCon);
	//		tremoloVocalsDSP.getType (out type);
	//		print (result);
	//		print (type);
	//		result = musicChanSubGroupDSP.getInput (2, out pitchDrumsDSP, out DSPCon);
	//		pitchDrumsDSP.getType (out type);
	//		print (result);
	//		print (type);
	//		result = musicChanSubGroupDSP.getInput (3, out pitchBassDSP, out DSPCon);
	//		pitchBassDSP.getType (out type);
	//		print (result);
	//		print (type);
	//		result = musicChanSubGroupDSP.getInput (4, out pitchLeadDSP, out DSPCon);
	//		pitchLeadDSP.getType (out type);
	//		print (result);
	//		print (type);
	//
	//		result = tremoloVocalsDSP.getInput (0, out flangerVocalsDSP, out DSPCon);
	//		result = flangerVocalsDSP.getType(out type);
	//		print(result);
	//		print(type);
	//
	//		result = flangerVocalsDSP.getInput (0, out pitchVocalsDSP, out DSPCon);
	//		result = pitchVocalsDSP.getType(out type);
	//		print(result);
	//		print(type);
	//
	//		/*pitchChordsDSP.setBypass(false);
	//		pitchVocalsDSP.setBypass(false);
	//		pitchDrumsDSP.setBypass(false);
	//		pitchBassDSP.setBypass(false);
	//		pitchLeadDSP.setBypass(false);*/
	//	}

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

		AudioStopMusic();

		PlayEjectSound ();
		PlayFlipAnimSound (switchedToAudioLog);

		// If we are in the audio log --> disable visuals
		if (switchedToAudioLog)
		{
			for (int i = 0; i < gameManager.overallCorruption.corruptedAreaList.Count; i++)
			{
				gameManager.overallCorruption.corruptedAreaList[i].SetActive(false);
			}
		}

		// If we are in the song --> enable visuals
		else if (!switchedToAudioLog)
		{
			for (int i = 0; i < gameManager.overallCorruption.corruptedAreaList.Count; i++)
			{
				gameManager.overallCorruption.corruptedAreaList[i].SetActive(true);
			}
		}

	}
}