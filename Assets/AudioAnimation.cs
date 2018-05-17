using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAnimation : MonoBehaviour {

	private AudioManager audioManager;

	void Start () {
		audioManager = FindObjectOfType<AudioManager> ();
	}

	public void PlayInsertAnimSound ()
	{
		FMOD.Studio.EventDescription insertCassetteEventDesc;
		FMOD.Studio.EventInstance insertCassetteEv;
		audioManager.systemObj.getEvent ("event:/SFX/insertCassette", out insertCassetteEventDesc);
		insertCassetteEventDesc.createInstance (out insertCassetteEv);
		insertCassetteEv.start ();
		insertCassetteEv.release ();
	}

	public void PlayFlipCassetteAnimSound()
	{
		
	}
}