using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ** Written by Hannes Gustafsson ** //

public class ButtonDisabler : MonoBehaviour {

    [Tooltip("Drag the buttons you wish to disable during 'recording' here")]
    [SerializeField] private List<Button> disableButtonList;
    //[SerializeField] private Dropdown dropdown;

    private AudioManager audioManager;
    private DrumMechanic drumMechanic;
    private FMOD.Studio.PLAYBACK_STATE playbackState;

	void Start ()
    {
        audioManager = GameManager.Instance.audioManager;
        drumMechanic = GameManager.Instance.drumMechanic.GetComponent<DrumMechanic>();
        Debug.Assert(disableButtonList.Count > 0, "Fill the list of button disabler with the buttons you desire to disable/enable when recording)");
	}
	

    public void DisableButtons()
    {
        audioManager.gameMusicEv.getPlaybackState(out playbackState);

        if(playbackState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            for (int i = 0; i < disableButtonList.Count; i++)
            {
                disableButtonList[i].interactable = false;
                //dropdown.interactable = false;
            }
        }

    }

    public void EnableButtons()
    {
        for (int i = 0; i < disableButtonList.Count; i++)
        {
            disableButtonList[i].interactable = true;
            //dropdown.interactable = true;
        }
    }


}
