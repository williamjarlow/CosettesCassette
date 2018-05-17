using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CassetteDisableButtons : MonoBehaviour {

    // Enables all buttons when the game object is enabled, which happens when the cassette animation is finished
    private void OnEnable()
    {
        GameManager.Instance.uiHandler.GetComponent<ButtonDisabler>().EnableButtons();
    }

}
