using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {


    private AudioMusic audioMusic;
	private AudioDistortion audioDistortion;
    private FMOD.Studio.EventDescription musicEventDesc;

    private int trackLength;
    [SerializeField] private string musicPath;
    [SerializeField] private string audioLogPath;

	void Awake (){
		audioMusic = GetComponent<AudioMusic> ();
		audioDistortion = GetComponent<AudioDistortion> ();

        musicEventDesc = FMODUnity.RuntimeManager.GetEventDescription(musicPath);
        musicEventDesc.getLength(out trackLength);

    }

	public void AudioPlayMusic (string musicTrack){
        //audioMusic = FindObjectOfType<AudioMusic>();
        audioMusic.PlayMusic ();
	}

	public void AudioStopMusic (){
		audioMusic.StopMusic ();
	}

	public void AudioPauseMusic (){
		audioMusic.PauseMusic ();
	}

	public void AudioUnpauseMusic (){
		audioMusic.UnpauseMusic ();
	}

    public float GetTrackLength()
    {
        return trackLength;
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
