using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AudioManager : MonoBehaviour
{

    [FMODUnity.EventRef]
    public FMOD.Studio.EventInstance gameMusicEv;

    private AudioMusic audioMusic;
    private AudioDistortion audioDistortion;
    private FMOD.Studio.EventDescription musicEventDesc;
    private FMOD.RESULT result;
    private FMOD.Studio.System systemObj;
    private FMOD.System lowLevelSys;

    private int trackLength;
    [SerializeField]
    private string musicPath;
    [SerializeField]
    private string audioLogPath;
    //Bank files to load, should only be the file name in the directory, eg. 'Cassette_01.bank'


    [SerializeField]
    private List<string> bankFiles;


    void Awake()
    {

        Debug.Assert(bankFiles.Count > 0, "Enter the bank file names into the audio manager");
        Debug.Assert(this.tag == "AudioManager", "Set the tag of AudioManager to 'AudioManager'");

        audioDistortion = GetComponent<AudioDistortion>();

        /*systemObj = FMODUnity.RuntimeManager.StudioSystem;
        lowLevelSys = FMODUnity.RuntimeManager.LowlevelSystem;

        //Attempts to reduce sound delay
        int numBuffers;
        uint bufferLength;

        //result = FMOD.Studio.System.create(out systemObj);
        //result = systemObj.getLowLevelSystem(out lowLevelSys);
        result = lowLevelSys.setSoftwareFormat(48000, FMOD.SPEAKERMODE.DEFAULT, 0);
        result = lowLevelSys.getDSPBufferSize(out bufferLength, out numBuffers);
        result = lowLevelSys.setDSPBufferSize(32, 2);
        result = lowLevelSys.getDSPBufferSize(out bufferLength, out numBuffers);
        Debug.Log(result);
        Debug.Log("After setting: " + bufferLength);
        Debug.Log("After setting: " + numBuffers);
        result = lowLevelSys.getDSPBufferSize(out bufferLength, out numBuffers);
        result = lowLevelSys.setOutput(FMOD.OUTPUTTYPE.NOSOUND);
        Debug.Log(result);
        result = systemObj.initialize(64, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
        Debug.Log("After init: " + bufferLength);
        Debug.Log("After init: " + numBuffers);*/


        //Loads the FMOD banks 
        for (int i = 0; i < bankFiles.Count; i++)
        {
            FMODUnity.RuntimeManager.LoadBank(bankFiles[i], true);
        }

        //Get the event description, needed to get Track Length
        musicEventDesc = FMODUnity.RuntimeManager.GetEventDescription(musicPath);
        musicEventDesc.getLength(out trackLength);


    }

    public void AudioPlayMusic()
    {
        gameMusicEv = FMODUnity.RuntimeManager.CreateInstance(musicPath);
        gameMusicEv.start();
        audioDistortion.UpdateDistortion();
    }

    public void AudioStopMusic()
    {
        gameMusicEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void AudioPauseMusic()
    {
        gameMusicEv.setPaused(true);
    }

    public void AudioUnpauseMusic()
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

    private void OnDestroy()
    {
        //Destroy systemObj because one is created each time in 'play' mode, and the unity editor does not destroy it for you when exiting 'play' mode
        systemObj.release();
        lowLevelSys.release();
    }


}