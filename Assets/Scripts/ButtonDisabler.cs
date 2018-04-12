using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDisabler : MonoBehaviour {

    [SerializeField] private List<Button> buttonList;
    [SerializeField] private Dropdown dropdown;

    private AudioManager audioManager;
    private FMOD.Studio.PLAYBACK_STATE playbackState;

	void Start ()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        Debug.Assert(buttonList.Count > 0, "Fill the list of button disabler with the buttons you desire to disable/enable when recording)");
	}
	

    public void DisableButtons()
    {
        audioManager.gameMusicEv.getPlaybackState(out playbackState);

        if(playbackState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            for (int i = 0; i < buttonList.Count; i++)
            {
                buttonList[i].interactable = false;
                dropdown.interactable = false;
            }
        }

    }

    public void EnableButtons()
    {
        for (int i = 0; i < buttonList.Count; i++)
        {
            buttonList[i].interactable = true;
            dropdown.interactable = true;
        }
    }
}
