using System;
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

	private AudioDistortion audioDistortion;

    private int trackLength;

    [SerializeField] private string musicPath;
    [SerializeField] private string audioLogPath;
    public string bassDrumPath;

    
    [Tooltip("Bank files to load, should only be the file name in the directory, eg. 'Cassette_01.bank'")]
    [SerializeField] private List<string> bankFiles;

    public bool switchedToAudioLog = false;

    void Awake ()
    {
        //Loads the FMOD banks
        for (int i = 0; i < bankFiles.Count; i++)
        {
            FMODUnity.RuntimeManager.LoadBank(bankFiles[i], true);
        }

        // Get the event description of music event, needed to get Track Length
        // Required to be in Awake() because alot of game objects ask for the track length in Start()
        musicEventDesc = FMODUnity.RuntimeManager.GetEventDescription(musicPath);
        musicEventDesc.getLength(out trackLength);
    }

    void Start()
    {
        Debug.Assert(bankFiles.Count > 0, "Enter the bank file names into the audio manager");
        Debug.Assert(this.tag == "AudioManager", "Set the tag of AudioManager to 'AudioManager'");

        audioDistortion = GetComponent<AudioDistortion>();

        //Used to retrieve DSP buffer size
        int numBuffers;
        uint bufferLength;

        //Creates FMOD system object and gets low-level system object
        FMOD.Studio.System.create(out systemObj);
        systemObj.getLowLevelSystem(out lowLevelSys);


        //FMOD system object initialization
        lowLevelSys.setSoftwareFormat(44100, FMOD.SPEAKERMODE.DEFAULT, 0);
        lowLevelSys.setDSPBufferSize(512, 4);
        lowLevelSys.setOutput(FMOD.OUTPUTTYPE.AUTODETECT);
        result = systemObj.initialize(64, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
    }

	public void AudioPlayMusic ()
    {
        if(!switchedToAudioLog)
            gameMusicEv = FMODUnity.RuntimeManager.CreateInstance(musicPath);
        if(switchedToAudioLog)
            gameMusicEv = FMODUnity.RuntimeManager.CreateInstance(audioLogPath);
        gameMusicEv.start();
    }

	public void AudioStopMusic ()
    {
        gameMusicEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

	public void AudioPauseMusic ()
    {
        gameMusicEv.setPaused(true);
    }

	public void AudioUnpauseMusic ()
    {
        gameMusicEv.setPaused(false);
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
