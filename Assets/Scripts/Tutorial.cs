using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class images
{
    public Image pageImage;
}

[System.Serializable]
class tutorial
{
    public Button tutorialButton;
    public List<images> images;
}

public class Tutorial : MonoBehaviour {

    [SerializeField] private GameObject tutorialMenu;
    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backwardsButton;
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;
    [SerializeField] private List<tutorial> tutorials;
    private int tutorialIndex = 0;
    private int imageIndex = 0;

    [SerializeField] private GameObject tutorialPrefab;
    [SerializeField] private TutorialStruct[] tutStruct;
    [SerializeField] private Button tutButton;
    [SerializeField] private AudioManager audioManager;

    private bool musicPaused = false;

    void Start()
    {
        //Add Listener to the button to leave the tutorial 
        Button tutorialButton = tutButton.GetComponent<Button>();
        tutorialButton.onClick.AddListener(() => Resume());

        tutorialPrefab.SetActive(false);
    }

    void Update()
    {
        for(int i = 0; i < tutStruct.Length; i++)
        {
            if (tutStruct[i].timeToAppear >= audioManager.GetTimeLinePosition() - 30 && tutStruct[i].timeToAppear <= audioManager.GetTimeLinePosition() + 30 && tutStruct[i].hasBeenShown == false)
            {
                TutorialEvent(i);
                tutStruct[i].hasBeenShown = true;
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
        tutorialImage.sprite = tutStruct[tutorialIndex].spriteToShow;
        Color tutAlpha = tutorialImage.color;
        tutAlpha.a = 0.75f;
        tutorialImage.color = tutAlpha;

        Time.timeScale = 0.0F;
        tutorialPrefab.SetActive(true);
        audioManager.AudioPauseMusic();
    }

    public void FlipPagesForward()
    {
        if (imageIndex <= tutorials[tutorialIndex].images.Count - 3)
        {
            //audioManager.PlayScriptFlip();
            imageIndex += 2;
            leftImage.sprite = tutorials[tutorialIndex].images[imageIndex].pageImage.sprite;
            if (imageIndex <= tutorials[tutorialIndex].images.Count - 2)
                rightImage.sprite = tutorials[tutorialIndex].images[imageIndex + 1].pageImage.sprite;
            if (imageIndex == tutorials[tutorialIndex].images.Count - 1)
                rightImage.sprite = null;
        }
    }

    public void FlipPagesBackwards()
    {
        if (imageIndex >= 1)
        {
            //audioManager.PlayScriptFlip();
            imageIndex -= 2;
            leftImage.sprite = tutorials[tutorialIndex].images[imageIndex].pageImage.sprite;
            rightImage.sprite = tutorials[tutorialIndex].images[imageIndex + 1].pageImage.sprite;
        }
    }

    public void BackToTutorials()
    {
        leftImage.sprite = null;
        rightImage.sprite = null;
        tutorialMenu.SetActive(true);
        forwardButton.SetActive(false);
        backwardsButton.SetActive(false);

        //audioManager.PlayPauseMenuBack();
    }

}

[System.Serializable]
public struct TutorialStruct
{
    public Sprite spriteToShow;
    public float timeToAppear;
    public bool hasBeenShown;
}
