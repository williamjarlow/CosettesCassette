using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//** Written by Hannes Gustafsson **//

public class OverallCorruptionBar : MonoBehaviour {

    private Image thisImage;
    private OverallCorruption overallCorruption;
    GameManager gameManager;
    private float corruptionPercentage;
    private float startValue;
    [SerializeField] private float speed = 1;
    //[SerializeField] Image completionBar;

    private void Awake()
    {
    }

    void Start ()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        thisImage = GetComponent<Image>();
        overallCorruption = gameManager.overallCorruption;

        //completionBar.fillAmount = (float)overallCorruption.corruptionClearThreshold/100;
        thisImage.fillAmount = 0;
	}
	
	void Update ()
    {
        corruptionPercentage = overallCorruption.overallCorruption;

        // Divide by 100 because fillAmount takes values from 0 --> 1 and overallCorrption has values from 0 --> 100
        thisImage.fillAmount = Mathf.Lerp(thisImage.fillAmount, 1 - overallCorruption.overallCorruption / 100, Time.deltaTime * speed);
        if (thisImage.fillAmount < (1 - overallCorruption.overallCorruption / 100) + 0.01f && thisImage.fillAmount > (1 - overallCorruption.overallCorruption / 100) - 0.01f)
            thisImage.fillAmount = 1 - overallCorruption.overallCorruption / 100;
    }
}
