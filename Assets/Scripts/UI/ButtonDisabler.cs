using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ** Written by Hannes Gustafsson ** //

public class ButtonDisabler : MonoBehaviour {

    [Tooltip("Drag the buttons you wish to disable during 'recording' here")]
    [SerializeField] private List<Button> disable2DButtonList;
    [Tooltip("The 3D buttons you wish to enable/disable")]
    [SerializeField] private List<ButtonController> disable3DButtonList;
    //[SerializeField] private Dropdown dropdown;

    private AudioManager audioManager;
    private DrumMechanic drumMechanic;
    private FMOD.Studio.PLAYBACK_STATE playbackState;

	void Start ()
    {
        audioManager = GameManager.Instance.audioManager;
        Debug.Assert(disable2DButtonList.Count > 0, "Fill the list of button disabler with the buttons you desire to disable/enable when recording)");
	}
	

    public void DisableButtons()
    {
        audioManager.gameMusicEv.getPlaybackState(out playbackState);


        // Disable 2D buttons
        for (int i = 0; i < disable2DButtonList.Count; i++)
        {
            disable2DButtonList[i].interactable = false;
        }

        // Disable 3D buttons
        for (int i = 0; i < disable3DButtonList.Count; i++)
        {
            disable3DButtonList[i].GetComponent<ButtonController>().enabled = !enabled;
        }

    }

    public void EnableButtons()
    {
        // Enable 2D buttons
        for (int i = 0; i < disable2DButtonList.Count; i++)
        {
            disable2DButtonList[i].interactable = true;
        }

        // Enable 3D buttons
        for(int i = 0; i < disable3DButtonList.Count; i++)
        {
            disable3DButtonList[i].GetComponent<ButtonController>().enabled = enabled;
        }
    }


}
