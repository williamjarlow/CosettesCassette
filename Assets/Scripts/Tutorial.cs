using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class images
{
    public Sprite pageImage;
}

[System.Serializable]
class tutorial
{
    public Button tutorialButton;
    public List<images> images;
}

public class Tutorial : MonoBehaviour {

    [SerializeField] private Text showText;
    [SerializeField] private GameObject tutorialMenu;
    [SerializeField] private GameObject tutorialScreen;
    [SerializeField] private GameObject settingsPage;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;
    [SerializeField] private GameObject book;
    [SerializeField] private List<tutorial> tutorials;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private TutorialStruct[] tutStruct;
    private int tutorialIndex = 0;
    private int imageIndex = 0;
    private bool musicPaused = false;
    private bool initializedFromGame = false;

    //[SerializeField] private GameObject tutorialPrefab;
    //[SerializeField] private Button tutButton;

    void Start()
    {
        ////Add Listener to the button to leave the tutorial 
        //Button tutorialButton = tutButton.GetComponent<Button>();
        //tutorialButton.onClick.AddListener(() => Resume());

        //tutorialPrefab.SetActive(false);
    }

    /*
    void Update()
    {
        for(int i = 0; i < tutorials.Count; i++)
        {
            if (tutStruct[i].timeToAppear >= audioManager.GetTimeLinePosition() - 30 && tutStruct[i].timeToAppear <= audioManager.GetTimeLinePosition() + 30 && tutStruct[i].hasBeenShown == false)
            {
                InitializeTutorialFromGame(i);
                tutStruct[i].hasBeenShown = true;
            }
        }
    }
    */

    ////Stops tutorial and resumes time
    //void Resume()
    //{
    //    Time.timeScale = 1.0F;
    //    tutorialPrefab.SetActive(false);
    //    audioManager.AudioUnpauseMusic();
    //}

    ////Call this funciton when you want to show tutorial with which sprite you want to use
    //public void TutorialEvent(int tutorialIndex)
    //{
    //    //Change base value of the chosen sprite's alpha to fit desired esthetic effect
    //    Image tutorialImage = tutorialPrefab.GetComponent<Image>();
    //    tutorialImage.sprite = tutStruct[tutorialIndex].spriteToShow;
    //    Color tutAlpha = tutorialImage.color;
    //    tutAlpha.a = 0.75f;
    //    tutorialImage.color = tutAlpha;

    //    Time.timeScale = 0.0F;
    //    tutorialPrefab.SetActive(true);
    //    audioManager.AudioPauseMusic();
    //}
    public void InitializeTutorialFromGame(int index)
    {
        tutorialIndex = index;
        tutorialMenu.SetActive(false);
        settingsPage.SetActive(false);
        tutorialScreen.SetActive(true);
        pauseMenu.SetActive(true);
        book.SetActive(true);
        imageIndex = 0;
        if (tutorials[index].images.Count > 0)
            leftImage.sprite = tutorials[index].images[0].pageImage;
        if (tutorials[index].images.Count > 1)
            rightImage.sprite = tutorials[index].images[1].pageImage;

        audioManager.AudioPauseMusic();
        initializedFromGame = true;
    }

    public void InitializeTutorial(int index)
    {
        tutorialIndex = index;
        tutorialMenu.SetActive(false);
        settingsPage.SetActive(false);
        tutorialScreen.SetActive(true);
        pauseMenu.SetActive(true);
        book.SetActive(true);
        imageIndex = 0;
        if (tutorials[index].images.Count > 0)
            leftImage.sprite = tutorials[index].images[0].pageImage;
        if (tutorials[index].images.Count > 1)
            rightImage.sprite = tutorials[index].images[1].pageImage;
        
        audioManager.PlayPauseMenuSelect();
    }

    public void FlipPagesForward()
    {
        if (imageIndex <= tutorials[tutorialIndex].images.Count - 3)
        {
            audioManager.PlayScriptFlip();
            imageIndex += 2;
            leftImage.sprite = tutorials[tutorialIndex].images[imageIndex].pageImage;
            if (imageIndex <= tutorials[tutorialIndex].images.Count - 2)
                rightImage.sprite = tutorials[tutorialIndex].images[imageIndex + 1].pageImage;
            if (imageIndex == tutorials[tutorialIndex].images.Count - 1)
                rightImage.sprite = null;
        }
        else if(initializedFromGame == true)
        {
            BackToGame();
        }
    }

    public void FlipPagesBackwards()
    {
        if (imageIndex >= 1)
        {
            audioManager.PlayScriptFlip();
            imageIndex -= 2;
            leftImage.sprite = tutorials[tutorialIndex].images[imageIndex].pageImage;
            rightImage.sprite = tutorials[tutorialIndex].images[imageIndex + 1].pageImage;
        }
    }

    public void BackToTutorials()
    {
        tutorialMenu.SetActive(true);
        book.SetActive(false);

        audioManager.PlayPauseMenuBack();
    }
    public void BackToGame()
    {
        tutorialMenu.SetActive(false);
        settingsPage.SetActive(true);
        tutorialScreen.SetActive(false);
        pauseMenu.SetActive(false);
        book.SetActive(false);
        initializedFromGame = false;

        audioManager.AudioUnpauseMusic();
    }

    public void TextboxShow(string text)
    {
        showText.text = text;
    }

}



[System.Serializable]
public struct TutorialStruct
{
    public Sprite spriteToShow;
    public float timeToAppear;
    public bool hasBeenShown;
}
