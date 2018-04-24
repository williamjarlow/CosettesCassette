using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMechanicChanger : MonoBehaviour {


    [SerializeField] private GameObject mechanics;
    private bool showingMechanics = false;

	void Start ()
    {

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
        GameManager.Instance.CurrentInstrument = (instruments)System.Enum.Parse(typeof(instruments), sentInstrument);
        mechanics.SetActive(false);
        showingMechanics = !showingMechanics;
    }


}
