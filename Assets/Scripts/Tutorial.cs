using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {

    [SerializeField] private GameObject tutorialPrefab;
    [SerializeField] private Sprite[] tutorialSprite;
    [SerializeField] private Button tutButton;

    void Start()
    {
        //Change base value of the chosen sprite's alpha to fit desired esthetic effect
        Image tutorialImage = tutorialPrefab.GetComponent<Image>();
        Color tutAlpha = tutorialImage.color;
        tutAlpha.a = 0.75f;
        tutorialImage.color = tutAlpha;

        //Add Listener to the button to leave the tutorial 
        Button tutorialButton = tutButton.GetComponent<Button>();
        tutorialButton.onClick.AddListener(() => Resume());

        tutorialPrefab.SetActive(false);
    }

    //Stops tutorial and resumes time
    void Resume()
    {
        Time.timeScale = 1.0F;
        tutorialPrefab.SetActive(false);
    }

    //Call this funciton when you want to show tutorial with which sprite you want to use
    public void TutorialEvent(int tutorialIndex)
    {
        tutorialImage.sprite = tutorialSprite[tutorialIndex];
        Time.timeScale = 0.0F;
        tutorialPrefab.SetActive(true);
    }
}
