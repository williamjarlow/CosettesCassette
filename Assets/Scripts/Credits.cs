using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Credits : MonoBehaviour {

    [SerializeField] private GameObject stopper;
    [SerializeField] private float stopPos;
    [SerializeField] private Vector3 freeze;
    [SerializeField] private float origPos;
    [SerializeField] public float currentPos;
    [SerializeField] private float endOfTheLine;
    [SerializeField] private GameObject AudioManagerz;
    [SerializeField] private GameObject Timeslider;
    [SerializeField] private GameObject credits;
    [SerializeField] private Image CreditsFade;
    [SerializeField] private Image AudiologPic;
    [SerializeField] private string creditsOrAudiolog;
    [SerializeField] private float waitforit;
    [SerializeField] private float safeguard;
    [SerializeField] private float delay;
    private AudioManager audioManager;
    private float audiolength;
    private float audiologlength;
    private float audioPos;
    private float audioP;
    public bool music = true;
    public bool audiolog = false;
    public bool showpicture = false;
    public bool showFade = false;
    private bool reachedEnd = false;







    // Use this for initialization
    void Start ()
    {

        audioManager = AudioManagerz.GetComponent<AudioManager>();
        audiolength = audioManager.GetTrackLength();
        print("Tracklength is " + audiolength);
        AudiologPic.CrossFadeAlpha(0, 0.0f, false);
        CreditsFade.CrossFadeAlpha(0, 0.0f, false);

        //insert load here
        AudiologPic.enabled = false;
        CreditsFade.enabled = false;
        if (showpicture == true)
        {
            AudiologPic.CrossFadeAlpha(0, 0.0f, false);
        }
        if (showFade == true)
        {
            CreditsFade.CrossFadeAlpha(0, 0.0f, false);
        }


    }

    // Update is called once per frame
    void Update ()
    {
        if (showpicture == true && audiolog == true)
        {
            AudiologPic.enabled = true;
        }
        else if (showpicture == true && audiolog == false)
        {
            AudiologPic.enabled = false;
        }
        if (creditsOrAudiolog == "Audiolog" && audiolog == true)
        {
            audioPos = audioManager.GetTimeLinePosition();
            audioP = audioPos / audiolength;
            currentPos = audioP * endOfTheLine;
            Vector3 temp2 = transform.localPosition;
            temp2.y = currentPos - origPos;
            transform.localPosition = temp2;
            if (currentPos < safeguard)
            {
                if (currentPos > waitforit && showpicture == false)
                {
                    Fade();
                    showpicture = true;
                    //insert save here
                }
            }

        }
        else if (creditsOrAudiolog == "Credits" && music == true)
        {
        
            //if (currentPos > waitforit && showFade == false)
            //{
                FadeOut();
                showFade = true;
                //insert save here
            //}

            if (currentPos < stopPos && reachedEnd == false)
            {
                print(audiolength);
                audioPos = audioManager.GetTimeLinePosition();
                print(audioPos);
                audioP = audioPos / audiolength;
                print(audioP);
                currentPos = audioP * endOfTheLine;
                print(endOfTheLine);
                Vector3 temp = transform.localPosition;
                temp.y = currentPos - origPos;
                transform.localPosition = temp;
                print(temp);
                print(currentPos-origPos);
            }
            else if (currentPos > stopPos && !reachedEnd)
            {
                transform.localPosition = freeze;
                reachedEnd = true;
                Timeslider.SetActive(false);
                //audioManager.AudioPauseMusic();
                //audioManager.pausedMusic = true;
            }
        }
    }
  
    public void Flip()
    {
        music = !music;
        audiolog = !audiolog;
    }

    public void Fade()
    {
        if (audiolog == true)
        {
            AudiologPic.enabled = true;
            StartCoroutine(Audiopic());
            
        }

        
    }
    public void FadeOut()
    {
        if (music == true)
        {
            CreditsFade.enabled = true;
            StartCoroutine(FadeOutCounter());

        }


    }

    IEnumerator Audiopic()
    {

        yield return new WaitForSeconds(delay);
        AudiologPic.CrossFadeAlpha(1, 1.0f, false);

    }

    IEnumerator FadeOutCounter()
    {

        yield return new WaitForSeconds(delay);
        CreditsFade.CrossFadeAlpha(0.5f, 0.0f, false);

    }

}

