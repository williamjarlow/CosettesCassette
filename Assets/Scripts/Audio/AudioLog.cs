using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLog : MonoBehaviour {


    [FMODUnity.EventRef]
    public FMOD.Studio.EventInstance audioLogEv;
    private AudioManager audioManager;


    private void Start()
    {
        audioManager = GameManager.Instance.audioManager;
    }

    public void PlayAudioLog()
    {
        audioLogEv = FMODUnity.RuntimeManager.CreateInstance(audioManager.GetAudioLogPath());
        audioLogEv.start();
    }

    public void StopAudioLog()
    {
        audioLogEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void PauseAudioLog()
    {
        audioLogEv.setPaused(true);
    }

    public void UnpauseAudioLog()
    {
        audioLogEv.setPaused(false);
    }

}
