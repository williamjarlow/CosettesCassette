using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMusic : MonoBehaviour {

	[FMODUnity.EventRef]
	public FMOD.Studio.EventInstance gameMusicEv;

	public void playMusic (string musicTrack){
		gameMusicEv = FMODUnity.RuntimeManager.CreateInstance (musicTrack);
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
