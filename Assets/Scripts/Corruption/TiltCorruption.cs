using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TiltCorruption : CorruptionBaseClass
{
    [SerializeField]
    private float mercyRange;
    [SerializeField]
    GameObject tiltIndicatorPrefab;
    [Range(0, 0.1f)]
    [SerializeField]
    private float moveSpeed;
    private AudioManager audioManager;
    private bool setPan = false;
    private float soundPos = 0;
    FMOD.RESULT result;
    FMOD.Studio.PLAYBACK_STATE state;

    GameObject tiltIndicatorInstance;

    OverallCorruption overallCorruption;

    void Start()
    {
        overallCorruption = GameManager.Instance.overallCorruption;
        audioManager = GameManager.Instance.audioManager;
        duration = overallCorruption.durations[segmentID];
    }

    void Update()
    {
        if (audioManager.GetTimeLinePosition() >= duration.start &&
            audioManager.GetTimeLinePosition() < duration.stop) //If player is inside a corrupted segment
        {
            if (inSegment == false) //If player just entered the segment
            {
                EnterSegment();
            }

            if (GameManager.Instance.recording) //If recording
            {
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
        if (GameManager.Instance.recording)
            corruptionClearedPercent = 0;
        tiltIndicatorInstance = Instantiate(tiltIndicatorPrefab);
        base.EnterSegment();
    }

    public override void ExitSegment()
    {
        //This function gets called upon when leaving the segment
        inSegment = false;
        if (GameManager.Instance.recording)
            GradeScore();
        corruptionClearedPercent = Mathf.Clamp(corruptionClearedPercent, 0, 100);
        innerDistortion = 0;
        audioManager.musicChanSubGroup.setPan(0);
        Destroy(tiltIndicatorInstance);
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
        //am.musicChanSubGroup.setPan(moveMusic);
        float x = Input.acceleration.x;
        if(soundPos + 0.05 > 0 && soundPos - 0.05 <= 0) //Replace this
        {
            soundPos += Random.Range(-0.03f, 0.03f); //Replace this
        }

        soundPos += soundPos / 100; //Replace this
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
        if(soundPos - mercyRange < 0 && soundPos + mercyRange > 0)
        {
            corruptionClearedPercent += 0.3f;
        }

        tiltIndicatorInstance.transform.GetChild(0).localPosition = new Vector3(soundPos*5, 0, 0);

    }

    void ResetConditions()
    {
        //You can use this function to reset any conditions that need to be reset upon leaving a segment.
    }
}
