using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class TiltCorruption : CorruptionBaseClass
{
    [SerializeField] [Range(0, 0.1f)] private float moveSpeed;
    [SerializeField] [Range(0, 1)] private float perfectRange;
    [Tooltip("Decides how quickly the sound will offset. ¨Higher value equals faster offset")]
    [SerializeField] [Range(1, 500)] private float offsetModifier;
    [SerializeField] private float windDuration;
    [SerializeField] private GameObject tiltIndicatorPrefab;
    private bool setPan = false;
    private float punishment;
    private float soundPos = 0;
    private const int startingScore = 100;
    FMOD.Studio.PLAYBACK_STATE state;

    private GameObject tiltIndicatorInstance;

    private AudioManager audioManager;

    private OverallCorruption overallCorruption;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
        overallCorruption = gameManager.overallCorruption;
        audioManager = gameManager.audioManager;
        duration = overallCorruption.durations[segmentID];
        Load();
    }

    void Update()
    {
        if (audioManager.GetTimeLinePosition() >= duration.start &&
            audioManager.GetTimeLinePosition() < duration.stop) //If player is inside a corrupted segment
        {


            if (gameManager.recording) //If recording
            {
                if (inSegment == false) //If player just entered the segment
                {
                    EnterSegment();
                }
                RecordSegment();
            }
        }
        else if (inSegment) //If player leaves the segment area
        {
            ExitSegment();
        }
    }

    public override void EnterSegment()
    {
        //This function gets called upon when entering the segment
        inSegment = true;
        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
        punishment = ((100 - clearThreshold) / (duration.stop - duration.start)) * 1000;
        if (gameManager.recording)
            corruptionClearedPercent = 0;
        tiltIndicatorInstance = Instantiate(tiltIndicatorPrefab, gameObject.transform);
        currentScore = startingScore;
        base.EnterSegment();
    }

    public override void ExitSegment()
    {
        //This function gets called upon when leaving the segment
        inSegment = false;
        if (gameManager.recording)
            GradeScore(); //Score gets graded and saved to file here
        corruptionClearedPercent = Mathf.Clamp(corruptionClearedPercent, 0, 100);
        innerDistortion = 0;
        audioManager.musicChanSubGroup.setPan(0);
        Destroy(tiltIndicatorInstance);
        Debug.Log(corruptionClearedPercent);
        base.ExitSegment();
        ResetConditions();
    }

    void RecordSegment()
    {
        audioManager.gameMusicEv.getPlaybackState(out state);
        if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING && setPan == false)
        {
            audioManager.musicChanGroup.setPan(0);
            audioManager.musicChanSubGroup.setPan(0);
            setPan = true;
        }
        else if (state != FMOD.Studio.PLAYBACK_STATE.STOPPED && setPan == true)
        {
            audioManager.musicChanGroup.setPan(0);
            audioManager.musicChanSubGroup.setPan(0);
            setPan = false;
        }

        float x = Input.acceleration.x;
        if (soundPos + 0.06f > 0 && soundPos - 0.06f <= 0)
        {
            if (soundPos > 0)
            {
                soundPos += Random.Range(0, 0.02f);
            }
            else if (soundPos < 0)
            {
                soundPos += Random.Range(-0.02f, 0);
            }
            else
            {
                soundPos += Random.Range(-0.03f, 0.03f);
            }
        }

        soundPos += soundPos * offsetModifier / 100;
        soundPos = Mathf.Clamp(soundPos, -1, 1);
        if (Input.GetKey("left"))
        {
            x = -1;
        }
        else if (Input.GetKey("right"))
        {
            x = 1;
        }

        if (x < -0.1f)
        {
            audioManager.musicChanSubGroup.setPan(Mathf.Clamp(soundPos - moveSpeed, -1, 1));
            soundPos = Mathf.Clamp(soundPos - moveSpeed, -1, 1);
        }
        else if (x > 0.1f)
        {
            audioManager.musicChanSubGroup.setPan(Mathf.Clamp(soundPos + moveSpeed, -1, 1));
            soundPos = Mathf.Clamp(soundPos + moveSpeed, -1, 1);
        }
        if (soundPos < -perfectRange || soundPos > perfectRange)
        {
            currentScore -= punishment * Time.deltaTime;
            Debug.Log(punishment);
        }
        
        if (soundPos <= -0.9f || soundPos >= 0.9f )
        {
            currentScore = 0;
            ExitSegment();
        }

        tiltIndicatorInstance.transform.GetChild(0).localPosition = new Vector3(soundPos * 5, 0, 0);

    }

    public override void GradeScore()
    {
        base.GradeScore();
    }

    void ResetConditions()
    {
        //You can use this function to reset any conditions that need to be reset upon leaving a segment.
    }
}