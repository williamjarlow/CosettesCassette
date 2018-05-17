using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ** Written by Hannes Gustafsson ** //

public class ButtonDisabler : MonoBehaviour {

    [Tooltip("The 3D buttons you wish to enable/disable")]
    [SerializeField] private List<ButtonController> disableButtonList;
    //[SerializeField] private Dropdown dropdown;

    GameManager gameManager;

    private AudioManager audioManager;
    private DrumMechanic drumMechanic;
    private FMOD.Studio.PLAYBACK_STATE playbackState;

    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

	void Start ()
    {
        audioManager = gameManager.audioManager;
        Debug.Assert(disableButtonList.Count > 0, "Fill the list of button disabler with the buttons you desire to disable/enable during tape switching animation)");
	}
	

    public void DisableButtons()
    {
        // Disable buttons
        for (int i = 0; i < disableButtonList.Count; i++)
        {
            disableButtonList[i].GetComponent<ButtonController>().isEnabled = false;
        }

    }

    public void EnableButtons()
    {
        // Enable buttons
        for(int i = 0; i < disableButtonList.Count; i++)
        {
            disableButtonList[i].GetComponent<ButtonController>().isEnabled = true;
        }
    }


}
