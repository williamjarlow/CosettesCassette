using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltMechanic : MonoBehaviour {

    [SerializeField] private float moveMusic;
    [SerializeField] GameObject audioMan;
    private AudioManager am;
    FMOD.RESULT result;

	// Use this for initialization
	void Start () {
        am = audioMan.GetComponent<AudioManager>();
	}
	
	// Update is called once per frame
	void Update () {
        //am.musicChanSubGroup.setPan(moveMusic);
        am.musicChanGroup.setPan(moveMusic);
    }
}
