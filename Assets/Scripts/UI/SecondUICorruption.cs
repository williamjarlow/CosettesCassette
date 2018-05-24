using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SecondUICorruption : MonoBehaviour {

    [SerializeField] private Image mainCorruptionBar;
    private Image secondaryCorruptionBar;


    void Start ()
    {
        secondaryCorruptionBar = GetComponent<Image>();
        secondaryCorruptionBar.sprite = mainCorruptionBar.sprite;
        secondaryCorruptionBar.color = mainCorruptionBar.color;
	}
	
	void Update ()
    {
		if (gameObject.activeSelf)
        {
            secondaryCorruptionBar.fillAmount = mainCorruptionBar.fillAmount;
        }
	}
}
