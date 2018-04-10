using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AudioManager : MonoBehaviour {

    [FMODUnity.EventRef]
    public FMOD.Studio.EventInstance gameMusicEv;

    private AudioMusic audioMusic;
	private AudioDistortion audioDistortion;
    private FMOD.Studio.EventDescription musicEventDesc;
    private FMOD.RESULT result;
    FMOD.Studio.System systemObj;
    FMOD.System lowLevelSys;

    private int trackLength;
    [SerializeField] private string musicPath;
    [SerializeField] private string audioLogPath;
    //Bank files to load, should only be the file name in the directory, eg. 'Cassette_01.bank'
    [SerializeField] private List<string> bankFiles;



    void Awake (){

        Debug.Assert(bankFiles.Count > 0, "Enter the bank file names into the audio manager");

        audioDistortion = GetComponent<AudioDistortion> ();

        //Attempts to reduce sound delay
        result = FMOD.Studio.System.create(out systemObj);
        result = systemObj.getLowLevelSystem(out lowLevelSys);
        result = lowLevelSys.setDSPBufferSize(512, 4);
        Debug.Log(result);
        result = lowLevelSys.setSoftwareFormat(44100, FMOD.SPEAKERMODE.DEFAULT, 0);
        result = lowLevelSys.setOutput(FMOD.OUTPUTTYPE.OPENSL);
        result = systemObj.initialize(64, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, (IntPtr)null);


        //Loads the banks 
        for (int i = 0; i < bankFiles.Count; i++)
        {
            FMODUnity.RuntimeManager.LoadBank(bankFiles[i], true);
        }

        //Get the event description, needed to get Track Length
        musicEventDesc = FMODUnity.RuntimeManager.GetEventDescription(musicPath);
        musicEventDesc.getLength(out trackLength);

        //Destroy systemObj because one is created each time in 'play' mode, and the unity editor does not destroy it for you when exiting 'play' mode
        systemObj.release();
        lowLevelSys.release();

    }

	public void AudioPlayMusic (){
        gameMusicEv = FMODUnity.RuntimeManager.CreateInstance(musicPath);
        gameMusicEv.start();
    }

	public void AudioStopMusic (){
        gameMusicEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

	public void AudioPauseMusic (){
        gameMusicEv.setPaused(true);
    }

	public void AudioUnpauseMusic (){
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


}
