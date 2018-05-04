using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltMechanic : MonoBehaviour {
    
    [Range(0, 0.1f)]
    [SerializeField] private float moveSpeed;
    private AudioManager am;
    private bool setPan = false;
    private float soundPos = 0;
    FMOD.RESULT result;
    FMOD.Studio.PLAYBACK_STATE state;

    // Use this for initialization
    void Start () {
        am = GameManager.Instance.audioManager;
    }
	
	// Update is called once per frame
	void Update ()
    {
        am.gameMusicEv.getPlaybackState(out state);
        if(state != FMOD.Studio.PLAYBACK_STATE.PLAYING && setPan == false)
        {
            am.musicChanGroup.setPan(0);
            am.musicChanSubGroup.setPan(0);
            setPan = true;
        }
        else if(state != FMOD.Studio.PLAYBACK_STATE.STOPPED && setPan == true)
        {
            am.musicChanGroup.setPan(0);
            am.musicChanSubGroup.setPan(0);
            setPan = false;
        }
        //am.musicChanSubGroup.setPan(moveMusic);
        //float x = Input.acceleration.x;

        float x = 0;
        if (Input.GetKey("left"))
        {
            x = -1;
        }
        else if (Input.GetKey("right"))
        {
            x = 1;
        }
        else
        {
            x = 0;
        }

        if (x < -0.1f)
        {
            am.musicChanSubGroup.setPan(Mathf.Clamp(soundPos - moveSpeed, -1, 1));
            soundPos = Mathf.Clamp(soundPos - moveSpeed, -1, 1);
        }
        else if (x > 0.1f)
        {
            am.musicChanSubGroup.setPan(Mathf.Clamp(soundPos + moveSpeed, -1, 1));
            soundPos = Mathf.Clamp(soundPos + moveSpeed, -1, 1);
        }
    }
}
