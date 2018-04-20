using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class Credits : MonoBehaviour {

    [SerializeField] private GameObject stopper;
    [SerializeField] private float origPos;
    [SerializeField] private float currentPos;
    [SerializeField] private float endOfTheLine;
    [SerializeField] private GameObject AudioManagerz;
    [SerializeField] private GameObject credits;
    private AudioManager audioManager;
    private float audiolength;
    private float audioPos;
    private float audioP;


    // Use this for initialization
    void Start ()
    {
        
        audioManager = AudioManagerz.GetComponent<AudioManager>();
        origPos = 1100f;
        endOfTheLine = 2260.116f;
        audiolength = audioManager.GetTrackLength();
        //FindString();
        //Text temp = credits.GetComponent<Text>();
        //temp.text = FindString();
        
    }

    // Update is called once per frame
    void Update ()
    {
        audioPos = audioManager.GetTimeLinePosition();
        audioP = audioPos / audiolength;
        currentPos = audioP * endOfTheLine;
        Vector3 temp = transform.localPosition;
        temp.y = currentPos - origPos;
        transform.localPosition = temp;

    }

    /*static string FindString()
    {
        string path = "Assets/Test_Folder/Credits.txt";

        StreamReader creditstext = new StreamReader(path);
        Debug.Log(creditstext.ReadToEnd());
        creditstext.Close();
        return creditstext.ToString();

    }*/
  
}

