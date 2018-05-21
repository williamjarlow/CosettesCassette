using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CassetteDisableButtons : MonoBehaviour {

    GameManager gameManager;

    // Enables all buttons when the game object is enabled, which happens when the cassette animation is finished
    private void OnEnable()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        gameManager.uiHandler.GetComponent<ButtonDisabler>().EnableButtons();
        
    }
}
