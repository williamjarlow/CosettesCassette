using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


    [System.Serializable]
    class pages
{
    [TextArea(3, 15)]
    public string pageText;
}

    [System.Serializable]
    class episodes
{
    public Button episodeButton;
    public string header;
    public List<pages> pages;
}

public class StoryTelling : MonoBehaviour {

    [SerializeField] private GameObject episodeMenu;
    [SerializeField] private Text transcriptHeader;
    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backwardsButton;
    [SerializeField] private Text leftPage;
    [SerializeField] private Text rightPage;
    [SerializeField] private List<episodes> episodes;
    private int episodeIndex = 0;
    private int pageIndex = 0;
    private string originalHeader = "";

	private AudioManager audioManager;

    void Start()
	{
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
        originalHeader = transcriptHeader.text;
        leftPage.text = "";
        rightPage.text = "";
    }

    public void InitializeStory(int index)
    {
        episodeIndex = index;
        transcriptHeader.text = episodes[index].header;
        episodeMenu.SetActive(false);
        forwardButton.SetActive(true);
        backwardsButton.SetActive(true);
        pageIndex = 0;
        if (episodes[index].pages.Count > 0)
            leftPage.text = episodes[index].pages[0].pageText;
        if (episodes[index].pages.Count > 1)
            rightPage.text = episodes[index].pages[1].pageText;

		audioManager.PlayPauseMenuSelect ();
    }

    public void FlipPagesForward()
    {
        if (pageIndex <= episodes[episodeIndex].pages.Count - 3)
        {
			audioManager.PlayScriptFlip ();
            pageIndex += 2;
                leftPage.text = episodes[episodeIndex].pages[pageIndex].pageText;
            if (pageIndex <= episodes[episodeIndex].pages.Count - 2)
                rightPage.text = episodes[episodeIndex].pages[pageIndex + 1].pageText;
            if (pageIndex == episodes[episodeIndex].pages.Count - 1)
                rightPage.text = "";
        }
    }

    public void FlipPagesBackwards()
    {
        if (pageIndex >= 1)
        {
			audioManager.PlayScriptFlip ();
            pageIndex -= 2;
            leftPage.text = episodes[episodeIndex].pages[pageIndex].pageText;
            rightPage.text = episodes[episodeIndex].pages[pageIndex + 1].pageText;
        }
    }

    public void BackToEpisodes()
    {
        transcriptHeader.text = originalHeader;
        leftPage.text = "";
        rightPage.text = "";
        episodeMenu.SetActive(true);
        forwardButton.SetActive(false);
        backwardsButton.SetActive(false);

		audioManager.PlayPauseMenuBack ();
    }


}
