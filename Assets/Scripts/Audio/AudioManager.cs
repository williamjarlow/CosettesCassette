using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AudioManager : MonoBehaviour {

	private FMOD.RESULT result;
	public FMOD.Studio.System systemObj;
	public FMOD.System lowLevelSys;

	public FMOD.DSP musicChanSubGroupDSP;
	public FMOD.DSP pitchChordsDSP;
	public FMOD.DSP pitchVocalsDSP;
	public FMOD.DSP pitchDrumsDSP;
	public FMOD.DSP pitchBassDSP;
	public FMOD.DSP pitchLeadDSP;

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

        //Loads the FMOD banks for the Unity Editor
        for (int i = 0; i < bankFiles.Count; i++)
        {
            FMODUnity.RuntimeManager.LoadBank(bankFiles[i] + ".bank", true);
			result = systemObj.getBank("bank:/" + bankFiles[i], out banks[i]);
        }

		FMODUnity.RuntimeManager.WaitForAllLoads ();
        systemObj.flushCommands ();

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
		
    public void AudioPlayMusic ()
    {
        if(!switchedToAudioLog)
			result = musicEventDesc.createInstance(out gameMusicEv);

        if(switchedToAudioLog)
			result = logEventDesc.createInstance(out gameMusicEv);

        result = gameMusicEv.start();

		if (!haveDSP) {
			StartCoroutine (GetDSP ());
			haveDSP = true;
		}
    }

	public void AudioStopMusic ()
    {
        result = gameMusicEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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

	private IEnumerator GetDSP()
	{
		FMOD.Studio.PLAYBACK_STATE state;

		gameMusicEv.getPlaybackState (out state);

		while (state != FMOD.Studio.PLAYBACK_STATE.PLAYING) 
		{
			gameMusicEv.getPlaybackState (out state);
			print("waiting for music to start");
			yield return null;
		}

		FMOD.ChannelGroup musicChanGroup;
		FMOD.ChannelGroup musicChanSubGroup;

		FMOD.DSPConnection DSPCon;

		result = gameMusicEv.getChannelGroup (out musicChanGroup);
		//print ("group: " + result);
		result = musicChanGroup.getGroup (0, out musicChanSubGroup);
		//print ("sub group: " + result);
		result = musicChanSubGroup.getDSP (3, out musicChanSubGroupDSP); // 3 carries channels of instrument tracks, 0 is the sum track
		//print ("sub group dsp: " + result);
		result = musicChanSubGroupDSP.getInput (0, out pitchChordsDSP, out DSPCon); // adding effects in studio messes up 4 and beyond
		//print ("dsp: " + result);
		result = musicChanSubGroupDSP.getInput (1, out pitchVocalsDSP, out DSPCon);
		//print ("dsp: " + result);
		result = musicChanSubGroupDSP.getInput (2, out pitchDrumsDSP, out DSPCon);
		//print ("dsp: " + result);
		result = musicChanSubGroupDSP.getInput (3, out pitchBassDSP, out DSPCon);
		//print ("dsp: " + result);
		result = musicChanSubGroupDSP.getInput (4, out pitchLeadDSP, out DSPCon);
		//print ("dsp: " + result);

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
        //Destroy FMOD system objects because they are created each time in 'play' mode, and the unity editor does not destroy them for you when exiting 'play' mode
        systemObj.release();
        lowLevelSys.release();
    }


}