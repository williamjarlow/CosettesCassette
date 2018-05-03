using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltMechanic : MonoBehaviour {

    [SerializeField] private float moveMusic;
    [SerializeField] private float moveSpeed;
    [SerializeField] GameObject audioMan;
    private AudioManager am;
    [SerializeField] private float temp = 0;
    FMOD.RESULT result;

	// Use this for initialization
	void Start () {
        am = audioMan.GetComponent<AudioManager>();
        am.musicChanGroup.setPan(0);
        am.musicChanSubGroup.setPan(0);
    }
	
	// Update is called once per frame
	void Update () {
        //am.musicChanSubGroup.setPan(moveMusic);
        float x = Input.acceleration.x;
        //float x = 0;
        //if (Input.GetKey("left"))
        //{
        //    x = -1;
        //}
        //else if (Input.GetKey("right"))
        //{
        //    x = 1;
        //}
        //else
        //{
        //    x = 0;
        //}

        //if (x < -0.1f)
        //{
        //    am.musicChanSubGroup.setPan(Mathf.Clamp(temp - moveSpeed, -1, 1));
        //    temp = Mathf.Clamp(temp - moveSpeed, -1, 1);
        //}
        //else if (x > 0.1f)
        //{
        //    am.musicChanSubGroup.setPan(Mathf.Clamp(temp + moveSpeed, -1, 1));
        //    temp = Mathf.Clamp(temp + moveSpeed, -1, 1);
        //}
    }
}
