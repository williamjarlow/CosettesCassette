using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CassetteDisableButtons : MonoBehaviour {

    GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    // Enables all buttons when the game object is enabled, which happens when the cassette animation is finished
    private void OnEnable()
    {
        gameManager.uiHandler.GetComponent<ButtonDisabler>().EnableButtons();
    }

    private void OnDisable()
    {
        gameManager.uiHandler.GetComponent<ButtonDisabler>().DisableButtons();
    }
}
