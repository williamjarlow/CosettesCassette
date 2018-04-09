using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private AudioSource currentSong;

    [Header("Song information needed for other scripts.")]
    public float pitch;
    public float posInSong;
    public float lengthOfSong;


	void Start ()
    {
        currentSong = GetComponent<AudioSource>();

        lengthOfSong = currentSong.clip.length;
    }

	void Update ()
    {
        pitch = currentSong.pitch;
        posInSong = currentSong.time;
	}
}
