using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMusic : MonoBehaviour {

	[FMODUnity.EventRef]
	public FMOD.Studio.EventInstance gameMusicEv;

    private AudioManager audioManager;


    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    public void PlayMusic  (){
        gameMusicEv = FMODUnity.RuntimeManager.CreateInstance(audioManager.GetMusicPath());
        gameMusicEv.start ();
	}

	public void StopMusic (){
		gameMusicEv.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	public void PauseMusic (){
		gameMusicEv.setPaused (true);
	}

	public void UnpauseMusic (){
		gameMusicEv.setPaused (false);
	}

    public float GetTimeLinePosition()
    {
        int temp;
        gameMusicEv.getTimelinePosition(out temp);
        return temp;
    }
}
