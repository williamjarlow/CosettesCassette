using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PitchType
{
    Chords,
    Vocals,
    Drums,
    Bass,
    Lead,
    All
}

public class AudioPitch : MonoBehaviour {

	private AudioManager audioManager;
    [SerializeField] private Slider pitchSlider;

	void Start (){
		audioManager = GetComponent<AudioManager> ();
	}
	
	// Update is called once per frame
	void Update ()
    {
    }

    public void SetPitch(float newPitch, PitchType pitchType)
    {
        switch (pitchType)
        {
            case PitchType.All:
                audioManager.gameMusicEv.setParameterValue("pitch_chords", newPitch);
                audioManager.gameMusicEv.setParameterValue("pitch_vocals", newPitch);
                audioManager.gameMusicEv.setParameterValue("pitch_drums", newPitch);
                audioManager.gameMusicEv.setParameterValue("pitch_bass", newPitch);
                audioManager.gameMusicEv.setParameterValue("pitch_lead", newPitch);
                break;
            case PitchType.Chords:
                audioManager.gameMusicEv.setParameterValue("pitch_chords", newPitch);
                break;
            case PitchType.Vocals:
                audioManager.gameMusicEv.setParameterValue("pitch_vocals", newPitch);
                break;
            case PitchType.Drums:
                audioManager.gameMusicEv.setParameterValue("pitch_drums", newPitch);
                break;
            case PitchType.Bass:
                audioManager.gameMusicEv.setParameterValue("pitch_bass", newPitch);
                break;
            case PitchType.Lead:
                audioManager.gameMusicEv.setParameterValue("pitch_lead", newPitch);
                break;

        }
    }

}
