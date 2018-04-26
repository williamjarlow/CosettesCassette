﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//** Written by Hannes Gustafsson **//

public class OverallCorruptionBar : MonoBehaviour {

    private Image thisImage;
    private OverallCorruption overallCorruption;
    private float corruptionPercentage;
    private float startValue;
    [SerializeField] private float speed = 1;

	void Start ()
    {
        thisImage = GetComponent<Image>();
        overallCorruption = GameManager.Instance.overallCorruption;
	}
	
	void Update ()
    {
        corruptionPercentage = overallCorruption.overallCorruption;

        // Divide by 100 because fillAmount takes values from 0 --> 1 and overallCorrption has values from 0 --> 100
        thisImage.fillAmount = Mathf.Lerp(thisImage.fillAmount, overallCorruption.overallCorruption / 100, Time.deltaTime * speed);

    }
}
