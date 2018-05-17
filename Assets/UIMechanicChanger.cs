using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMechanicChanger : MonoBehaviour {


    [SerializeField] private GameObject mechanics;
    private bool showingMechanics = false;

    GameManager gameManager;

    void Start ()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }


    public void ToggleShowMechanics()
    {
        showingMechanics = !showingMechanics;
        if (showingMechanics)
            mechanics.SetActive(true);
        if (!showingMechanics)
            mechanics.SetActive(false);
    }


    public void ChangeCurrentImage(string sentInstrument)
    {
        gameObject.GetComponent<Image>().sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
        gameManager.SelectedInstrument = (instruments)System.Enum.Parse(typeof(instruments), sentInstrument);
        mechanics.SetActive(false);
        showingMechanics = !showingMechanics;
    }


}
