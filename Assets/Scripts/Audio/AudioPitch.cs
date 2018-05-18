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

	void Update ()
    {
    }

    public void SetPitch(float newPitch, PitchType pitchType)
    {
        switch (pitchType)
        {
            case PitchType.All:
				audioManager.pitchChords.setValue(newPitch);
				audioManager.pitchVocals.setValue(newPitch);
				audioManager.pitchDrums.setValue(newPitch);
				audioManager.pitchBass.setValue(newPitch);
				audioManager.pitchLead.setValue(newPitch);
                break;
            case PitchType.Chords:
				audioManager.pitchChords.setValue(newPitch);
                break;
            case PitchType.Vocals:
				audioManager.pitchVocals.setValue(newPitch);
                break;
            case PitchType.Drums:
				audioManager.pitchDrums.setValue(newPitch);
                break;
            case PitchType.Bass:
				audioManager.pitchBass.setValue(newPitch);
                break;
            case PitchType.Lead:
				audioManager.pitchLead.setValue(newPitch);
                break;
        }

		print (newPitch);
    }

	public void TogglePitch(PitchType pitchType, float bypassState)
	{
		switch (pitchType)
		{
		case PitchType.All:
			audioManager.togglePitchVocals.setValue (bypassState);
			audioManager.togglePitchChords.setValue (bypassState);
			audioManager.togglePitchDrums.setValue (bypassState);
			audioManager.togglePitchBass.setValue (bypassState);
			audioManager.togglePitchLead.setValue (bypassState);
			break;
		case PitchType.Chords:
			audioManager.togglePitchChords.setValue (bypassState);
			break;
		case PitchType.Vocals:
			audioManager.togglePitchVocals.setValue (bypassState);
			break;
		case PitchType.Drums:
			audioManager.togglePitchDrums.setValue (bypassState);
			break;
		case PitchType.Bass:
			audioManager.togglePitchBass.setValue (bypassState);
			break;
		case PitchType.Lead:
			audioManager.togglePitchLead.setValue (bypassState);
			break;
		}
	}

}