using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class LiveTutorials
{
    [TextArea]
    [SerializeField]
    public string tutorialText;
    public float timeToAppearInMs;
    public bool specialCaseWithDelay = false;
    public float delayedCaseInSeconds = 3.5f;
    [HideInInspector] public bool hasBeenShown = false;
}

public class LiveTutorial : MonoBehaviour {


    [SerializeField] private GameObject liveTutorial;
    [SerializeField] private Text textBoxForLiveTutorial;
    [SerializeField] public LiveTutorials[] liveTutorials;
    private AudioManager audioManager;
    private float tolerance = 120;


    void Start ()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
	}

    void Update()
    {
        if (liveTutorials.Length > 0)
        {
            for (int i = 0; i < liveTutorials.Length; i++)
            {
                if (liveTutorials[i].timeToAppearInMs >= audioManager.GetTimeLinePosition() - tolerance && liveTutorials[i].timeToAppearInMs <= audioManager.GetTimeLinePosition() + tolerance && liveTutorials[i].hasBeenShown == false)
                {
                    if (liveTutorials[i].specialCaseWithDelay)
                    {
                        liveTutorials[i].hasBeenShown = true;
                        StartCoroutine(delayedSpecialCase(liveTutorials[i].delayedCaseInSeconds, liveTutorials[i].tutorialText));
                    }
                    else
                    {
                        OpenLiveTutorial();
                        textBoxForLiveTutorial.gameObject.SetActive(true);
                        textBoxForLiveTutorial.text = liveTutorials[i].tutorialText;
                        liveTutorials[i].hasBeenShown = true;
                    }
                }
            }
        }
    }


    private void OpenLiveTutorial()
    {
		audioManager.PlayTutorialOpen();
        liveTutorial.SetActive(true);
        audioManager.gameMusicEv.setPaused(true);
        audioManager.pausedMusic = true;
    }

    public void CloseLiveTutorial()
    {
		audioManager.PlayTutorialClose();
        liveTutorial.SetActive(false);
        audioManager.gameMusicEv.setPaused(false);
        audioManager.pausedMusic = false;
    }


    public void ForceOpenLiveTutorial(string textToShow)
    {
        audioManager.PlayTutorialOpen();
        liveTutorial.SetActive(true);
        audioManager.gameMusicEv.setPaused(true);
        textBoxForLiveTutorial.text = textToShow;
    }

    public void ForceOpenLiveTutorial(string textToShow, float delay)
    {
        StartCoroutine(delayedSpecialCase(delay, textToShow));
    }

    IEnumerator delayedSpecialCase(float delay, string textToShow)
    {
        yield return new WaitForSeconds(delay);
        OpenLiveTutorial();
        textBoxForLiveTutorial.gameObject.SetActive(true);
        textBoxForLiveTutorial.text = textToShow;
    }

}
