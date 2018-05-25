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
class textPage
{
    [TextArea(3, 15)]
    public string pageText;
}

[System.Serializable]
class tutorial
{
    public Button tutorialButton;
    public string header;
    public List<images> images;
    public List<textPage> textPages;
}

public class Tutorial : MonoBehaviour
{

    [SerializeField] private GameObject tutorialMenu;
    [SerializeField] private Image leftImage;
    [SerializeField] private Text rightTextBox;
    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backwardsButton;
    [SerializeField] private GameObject book;
    [SerializeField] private Text tutorialHeader;
    [SerializeField] private List<tutorial> tutorials;
    private AudioManager audioManager;
    private int tutorialIndex = 0;
    private int currentIndex = 0;
    private string originalHeader = "";

    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        originalHeader = tutorialHeader.text;
    }



    public void InitializeTutorial(int index)
    {
        tutorialIndex = index;
        tutorialHeader.text = tutorials[index].header;
        tutorialMenu.SetActive(false);
        book.SetActive(true);
        if (tutorials[index].images.Count > 1)
            forwardButton.SetActive(true);
        currentIndex = 0;
        if (tutorials[index].images.Count > 0 && tutorials[index].textPages.Count > 0)
        {
            leftImage.sprite = tutorials[index].images[0].pageImage;
            rightTextBox.text = tutorials[index].textPages[0].pageText;
        }
        audioManager.PlayPauseMenuSelect();
    }

    public void FlipPagesForward()
    {
        if (currentIndex < tutorials[tutorialIndex].images.Count - 1)
        {
            audioManager.PlayScriptFlip();
            currentIndex += 1;
            leftImage.sprite = tutorials[tutorialIndex].images[currentIndex].pageImage;
            rightTextBox.text = tutorials[tutorialIndex].textPages[currentIndex].pageText;
        }
        if (currentIndex > 0)
            backwardsButton.SetActive(true);
        if (currentIndex == tutorials[tutorialIndex].images.Count - 1)
            forwardButton.SetActive(false);
    }

    public void FlipPagesBackwards()
    {
        if (currentIndex >= 1)
        {
            audioManager.PlayScriptFlip();
            currentIndex -= 1;
            leftImage.sprite = tutorials[tutorialIndex].images[currentIndex].pageImage;
            rightTextBox.text = tutorials[tutorialIndex].textPages[currentIndex].pageText;
        }
        if (currentIndex < tutorials[tutorialIndex].images.Count - 1)
            forwardButton.SetActive(true);
        if (currentIndex == 0)
            backwardsButton.SetActive(false);
    }

    public void BackToTutorials()
    {
        leftImage.sprite = null;
        rightTextBox.text = "";
        tutorialHeader.text = originalHeader;
        tutorialMenu.SetActive(true);
        book.SetActive(false);
        forwardButton.SetActive(false);
        backwardsButton.SetActive(false);
        audioManager.PlayPauseMenuBack();
    }
}