using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [FMODUnity.EventRef]
    public FMOD.Studio.EventInstance gameMusicEv;

    private AudioMusic audioMusic;
	private AudioDistortion audioDistortion;
    private FMOD.Studio.EventDescription musicEventDesc;

    private int trackLength;
    [SerializeField] private string musicPath;
    [SerializeField] private string audioLogPath;

	void Start (){
		audioMusic = GetComponent<AudioMusic> ();
		audioDistortion = GetComponent<AudioDistortion> ();

        musicEventDesc = FMODUnity.RuntimeManager.GetEventDescription(musicPath);
        musicEventDesc.getLength(out trackLength);
    }

	public void AudioPlayMusic (string musicTrack){
        gameMusicEv = FMODUnity.RuntimeManager.CreateInstance(musicPath);
        gameMusicEv.start();
        audioDistortion.UpdateDistortion();
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
