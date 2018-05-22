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
    public bool hasBeenShown;
}

public class LiveTutorial : MonoBehaviour {


    [SerializeField] private GameObject liveTutorial;
    [SerializeField] private Text textBoxForLiveTutorial;

    [SerializeField] public LiveTutorials[] liveTutorials;

    private AudioManager audioManager;


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
                if (liveTutorials[i].timeToAppearInMs >= audioManager.GetTimeLinePosition() - 30 && liveTutorials[i].timeToAppearInMs <= audioManager.GetTimeLinePosition() + 30 && liveTutorials[i].hasBeenShown == false)
                {
                    OpenLiveTutorial();
                    textBoxForLiveTutorial.gameObject.SetActive(true);
                    textBoxForLiveTutorial.text = liveTutorials[i].tutorialText;
                    liveTutorials[i].hasBeenShown = true;
                }
            }
        }
    }


    private void OpenLiveTutorial()
    {
        liveTutorial.SetActive(true);
        audioManager.AudioPauseMusic();
        audioManager.pausedMusic = true;
    }

    public void CloseLiveTutorial()
    {
        liveTutorial.SetActive(false);
        audioManager.AudioUnpauseMusic();
        audioManager.pausedMusic = false;
    }




}
