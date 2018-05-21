using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class TiltCorruption : CorruptionBaseClass
{
    [SerializeField] [Range(0, 0.1f)] private float moveSpeed;
    [SerializeField] [Range(0, 1)] private float spikeArea;
    [SerializeField] [Range(0, 1)] private float perfectRange;
    [Tooltip("Decides how quickly the sound will offset. ¨Higher value equals faster offset")]
    //[SerializeField] [Range(1, 500)] private float offsetModifier;
    [SerializeField] [Range(0, 100)] private float minumumScore;
    [Tooltip("Decides how strong the wind is. ¨Higher value equals stronger wind")]
    [SerializeField] [Range(0, 0.1f)] private float windSpeed;
    [SerializeField] private float randomWindDuration;
    [SerializeField] private float randomWindSpawnLowerBound;
    [SerializeField] private float randomWindSpawnUpperBound;
    [SerializeField] private GameObject tiltIndicatorPrefab;
    [SerializeField] private bool useRandomWind;
    [SerializeField] private WindStruct[] winds;
    private bool setPan = false;
    //private bool windIsBlowing = false;
    private float punishment;
    private float soundPos = 0;
    private float RNGWindSpawner;
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
        RNGWindSpawner = Random.Range(randomWindSpawnLowerBound, randomWindSpawnUpperBound);
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
        punishment = ((float)(100000 - minumumScore) / (duration.stop - duration.start));
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
        base.ExitSegment();
        ResetConditions();
    }

    void RecordSegment()
    {
        //Reset Panning on entering and exiting segment
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

        //Check for acceleromator and update x based on it
        float x = Input.acceleration.x;

        ////Edgecase handler for values close to 0
        //if (soundPos + 0.06f > 0 && soundPos - 0.06f <= 0 && windIsBlowing ==false)
        //{
        //    if (soundPos > 0)
        //    {
        //        soundPos += Random.Range(0, 0.02f);
        //    }
        //    else if (soundPos < 0)
        //    {
        //        soundPos += Random.Range(-0.02f, 0);
        //    }
        //    else
        //    {
        //        soundPos += Random.Range(-0.03f, 0.03f);
        //    }
        //}

        ////Offset position over time
        //soundPos += soundPos * offsetModifier / 100;
        //soundPos = Mathf.Clamp(soundPos, -1, 1);

        //Keyboard functions
        if (Input.GetKey("left"))
        {
            x = -1;
        }
        else if (Input.GetKey("right"))
        {
            x = 1;
        }

        //Move sound and indicator based on x value, x is either deciedd by keyboard or acceleromator
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

        //Lose points if not inside perfect range
        if (soundPos < -perfectRange || soundPos > perfectRange)
        {
            currentScore -= punishment * Time.deltaTime;
        }

        //Lose minigame if outside of boundaries
        if (soundPos <= -spikeArea || soundPos >= spikeArea)
        {
            currentScore = 0;
            gameManager.recording = false;
            ExitSegment();
        }

        //Random wind handler
        RNGWindSpawner -= Time.deltaTime;
        if (RNGWindSpawner <= 0 && useRandomWind == true)
        {
            float randomDirection = Random.Range(0f, 1f);
            Debug.Log(randomDirection);
            if(randomDirection < 0.5f)
            {
                StartCoroutine(WindEffect(windSpeed, false, randomWindDuration));
            }
            else
            {
                StartCoroutine(WindEffect(windSpeed, true, randomWindDuration));
            }

            RNGWindSpawner = Random.Range(randomWindSpawnLowerBound, randomWindSpawnUpperBound);
        }

        //Preset wind handler
        if(useRandomWind == false)
        {
            for (int i = 0; i < winds.Length; i++)
            {
                if (duration.start + winds[i].timeToAppear >= audioManager.GetTimeLinePosition() - 30 && duration.start + winds[i].timeToAppear <= audioManager.GetTimeLinePosition() + 30 && winds[i].hasBeenActive == false)
                {
                    if(winds[i].blowLeft == true)
                    {
                        StartCoroutine(WindEffect(windSpeed, winds[i].blowLeft, winds[i].duration));
                    }
                    else
                    {
                        StartCoroutine(WindEffect(windSpeed, winds[i].blowLeft, winds[i].duration));
                    }
                    winds[i].hasBeenActive = true;
                }
            }
        }
        
        tiltIndicatorInstance.transform.GetChild(0).localPosition = new Vector3(soundPos * 5, 0, 0);
    }

    public override void GradeScore()
    {
        base.GradeScore();
    }

    void ResetConditions()
    {
        soundPos = 0;
        for(int i = 0; i < winds.Length; i++)
        {
            winds[i].hasBeenActive = false;
        }
    }

    IEnumerator WindEffect(float strength, bool blowLeft, float time)
    {
        //windIsBlowing = true;
        bool moving = false;
        if (!moving)
        {                     // Do nothing if already moving
            moving = true;                 // Set flag to true
            float t = 1.0f;
            while (t >= 0.0f)
            {
                t -= Time.deltaTime / time; // Sweeps from 0 to 1 in time seconds
                Debug.Log(blowLeft);
                if(blowLeft == true)
                {
                    soundPos -= strength;
                    tiltIndicatorInstance.transform.GetChild(1).gameObject.SetActive(true);
                    tiltIndicatorInstance.transform.GetChild(1).localPosition = new Vector3(1f, 0, 0);
                }
                else
                {
                    soundPos += strength;
                    tiltIndicatorInstance.transform.GetChild(1).gameObject.SetActive(true);
                    tiltIndicatorInstance.transform.GetChild(1).localPosition = new Vector3(-1f, 0, 0);
                }

                yield return new WaitForEndOfFrame();
            }
            tiltIndicatorInstance.transform.GetChild(1).gameObject.SetActive(false);
            //windIsBlowing = false;
        }
    }
}
[System.Serializable]
public struct WindStruct
{
    public float timeToAppear;
    public float duration;
    public bool blowLeft;
    [HideInInspector] public bool hasBeenActive;
}