using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {

    [SerializeField] private GameObject tutorialPrefab;
    [SerializeField] private Sprite[] tutorialSprite;
    [SerializeField] private float[] timeToAppear;
    [SerializeField] private Button tutButton;
    [SerializeField] private AudioManager audioManager;

    void Start()
    {
        //Add Listener to the button to leave the tutorial 
        Button tutorialButton = tutButton.GetComponent<Button>();
        tutorialButton.onClick.AddListener(() => Resume());

        tutorialPrefab.SetActive(false);
    }

    void Update()
    {
        for(int i = 0; i < tutorialSprite.Length; i++)
        {
            if (timeToAppear[i] >= audioManager.GetTimeLinePosition() - 30 && timeToAppear[i] <= audioManager.GetTimeLinePosition() + 30 )
            {
                TutorialEvent(i);
            }
        }
    }

    //Stops tutorial and resumes time
    void Resume()
    {
        Time.timeScale = 1.0F;
        tutorialPrefab.SetActive(false);
        audioManager.AudioUnpauseMusic();
    }

    //Call this funciton when you want to show tutorial with which sprite you want to use
    public void TutorialEvent(int tutorialIndex)
    {
        //Change base value of the chosen sprite's alpha to fit desired esthetic effect
        Image tutorialImage = tutorialPrefab.GetComponent<Image>();
        tutorialImage.sprite = tutorialSprite[tutorialIndex];
        Color tutAlpha = tutorialImage.color;
        tutAlpha.a = 0.75f;
        tutorialImage.color = tutAlpha;

        Time.timeScale = 0.0F;
        audioManager.AudioPauseMusic();
        tutorialPrefab.SetActive(true);
    }
}
