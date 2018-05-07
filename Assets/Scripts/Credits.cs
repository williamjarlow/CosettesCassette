using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Credits : MonoBehaviour {

    [SerializeField] private GameObject stopper;
    [SerializeField] private float origPos;
    [SerializeField] public float currentPos;
    [SerializeField] private float endOfTheLine;
    [SerializeField] private GameObject AudioManagerz;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject winning;
    [SerializeField] private Image AudiologPic;
    private AudioManager audioManager;
    private KillScreen KS;
    private float audiolength;
    private float audiologlength;
    private float audioPos;
    private float audioP;
    public bool music = true;
    public bool audiolog = false;
    [SerializeField] private string creditsOrAudiolog;
    [SerializeField] private float waitforit;


    // Use this for initialization
    void Start ()
    {
        AudiologPic.CrossFadeAlpha(0, 0.0f, false);
        KS = winning.GetComponent<KillScreen>();
        //audiolog = GameManager.Instance.GetComponent<AudioManager>().switchedToAudioLog;
        audioManager = AudioManagerz.GetComponent<AudioManager>();
        //origPos = 2200f;
        //endOfTheLine = 4400f;
        audiolength = audioManager.GetTrackLength();

    }

    // Update is called once per frame
    void Update ()
    {
        if (creditsOrAudiolog == "Audiolog" && audiolog ==true)
        {
            /*print("works");
            audioPos = audioManager.GetTimeLinePosition();
            audioP = audioPos / audiolength;
            currentPos = audioP * endOfTheLine;
            Vector3 temp2 = transform.localPosition;
            temp2.y = currentPos - origPos;
            transform.localPosition = temp2;
            */

        }
        else if (creditsOrAudiolog == "Credits" && music == true)
        {
            audioPos = audioManager.GetTimeLinePosition();
            audioP = audioPos / audiolength;
            currentPos = audioP * endOfTheLine;
            Vector3 temp = transform.localPosition;
            temp.y = currentPos - origPos;
            transform.localPosition = temp;
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

            StartCoroutine(Audiopic());
            
        }
        else
        {

        }
        
    }

    IEnumerator Audiopic()
    {

        yield return new WaitForSeconds(waitforit);
        AudiologPic.CrossFadeAlpha(1, 1.0f, false);
    }

}

