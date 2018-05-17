using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownHandler : MonoBehaviour {

    // If the order of these lists are changed, the function SelectInstrument() must also be changed to achieve the correct function
    // Unfortunately there does not seem to be another way other than hardcoding the functions based on the dropdown.value (which dropdown is selected)
    private List<string> instrumentParams = new List<string>() { "solo_drums", "solo_vocals", "solo_lead", "solo_chords", "solo_bass" };
    private List<string> dropdownNames = new List<string>() { "All instruments", "Drums", "Vocals", "Melody", "Chords", "Bass" };
    [SerializeField] private Dropdown dropdown;

    GameManager gameManager;

    private AudioManager audioManager;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Start ()
    {
        audioManager = gameManager.audioManager;
        Debug.Assert(dropdown != null, "Attach the dropdown to the dropdown handler");

        dropdown.AddOptions(dropdownNames);

	}


    public void SelectInstrument()
    {
        // Turn off 'solo' on all tracks
        for (int i = 0; i < instrumentParams.Count; i++)
        {
            audioManager.gameMusicEv.setParameterValue(instrumentParams[i], 0);
        }

        switch (dropdown.value)
        {

            case 0:
                break;

                //Setting the parameter value of eg. "solo_drums" to 1 soloes the drums, meaning only the drums are audible
            case 1:

                audioManager.gameMusicEv.setParameterValue(instrumentParams[0], 1);
                break;

            case 2:
                audioManager.gameMusicEv.setParameterValue(instrumentParams[1], 1);
                break;

            case 3:
                audioManager.gameMusicEv.setParameterValue(instrumentParams[2], 1);
                break;

            case 4:
                audioManager.gameMusicEv.setParameterValue(instrumentParams[3], 1);
                break;

            case 5:
                audioManager.gameMusicEv.setParameterValue(instrumentParams[4], 1);
                Debug.Log("Bass");
                break;
        } 
    }
}
