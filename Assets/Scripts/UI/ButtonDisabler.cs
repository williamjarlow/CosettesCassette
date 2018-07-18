using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ** Written by Hannes Gustafsson ** //

public class ButtonDisabler : MonoBehaviour {

    [Tooltip("The 3D buttons you wish to enable/disable")]
    [SerializeField] private List<GameObject> disableButtonList;
    //[SerializeField] private Dropdown dropdown;

    GameManager gameManager;

    private AudioManager audioManager;
    private DrumMechanic drumMechanic;
    private FMOD.Studio.PLAYBACK_STATE playbackState;

	void Start ()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        audioManager = gameManager.audioManager;
        Debug.Assert(disableButtonList.Count > 0, "Fill the list of button disabler with the buttons you desire to disable/enable during tape switching animation)");
	}
	
    public void ToggleSkipButtons()
    {
        disableButtonList[1].GetComponent<Button>().interactable = !disableButtonList[1].GetComponent<Button>().interactable;
        disableButtonList[2].GetComponent<Button>().interactable = !disableButtonList[2].GetComponent<Button>().interactable;
    }

    public void ToggleRepairButton()
    {
        disableButtonList[3].GetComponent<Button>().interactable = !disableButtonList[3].GetComponent<Button>().interactable;
    }

    public void ToggleButtonsInteractable()
    {
        for (int i = 0; i < disableButtonList.Count; i++)
        {
            disableButtonList[i].GetComponent<Button>().interactable = !disableButtonList[i].GetComponent<Button>().interactable;
        }
    }

    public void DisableButtons()
    {
        Debug.Log("REEEEE");
        // Disable buttons
        for (int i = 0; i < disableButtonList.Count; i++)
        {
            disableButtonList[i].GetComponent<Button>().interactable = false;
        }

    }

    public void EnableButtons()
    {
        // Enable buttons
        for(int i = 0; i < disableButtonList.Count; i++)
        {
            disableButtonList[i].GetComponent<Button>().interactable = true;
        }
    }


}
