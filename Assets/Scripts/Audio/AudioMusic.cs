using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMusic : MonoBehaviour {

	[FMODUnity.EventRef]
	public FMOD.Studio.EventInstance gameMusicEv;

    // Path for the music that alternates, ie. the music that is being distorted, change of pitch, timeline, etc
    public string musicPath;

	public void playMusic  (){
		gameMusicEv = FMODUnity.RuntimeManager.CreateInstance (musicPath);
		gameMusicEv.start ();
	}

	public void stopMusic (){
		gameMusicEv.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	public void pauseMusic (){
		gameMusicEv.setPaused (true);
	}

	public void unpauseMusic (){
		gameMusicEv.setPaused (false);
	}
}
