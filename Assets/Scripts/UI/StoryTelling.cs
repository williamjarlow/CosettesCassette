using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    [System.Serializable]
    class pages
{
    [TextArea(3, 10)]
    public string pageText;
}

    [System.Serializable]
    class episodes
{
    [SerializeField] public List<pages> pages;
}

public class StoryTelling : MonoBehaviour {

    [SerializeField] private Text leftPage;
    [SerializeField] private Text rightPage;
    [SerializeField] private List<episodes> episodes;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            FlipPagesForward();
        if (Input.GetKeyDown(KeyCode.J))
            FlipPagesBackwards();
    }

    private void InitializeStory()
    {

    }

    private void FlipPagesForward()
    {
        leftPage.text = episodes[0].pages[2].pageText;
        rightPage.text = episodes[0 ].pages[3].pageText;
    }

    private void FlipPagesBackwards()
    {
        leftPage.text = episodes[0].pages[0].pageText;
        rightPage.text = episodes[0].pages[1].pageText;
    }


}
