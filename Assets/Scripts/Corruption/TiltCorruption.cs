using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class TiltCorruption : CorruptionBaseClass
{
    [SerializeField] List<ScoreAreaNode> nodes;
    [SerializeField] [Range(0, 0.1f)] private float moveSpeed;
    [SerializeField] [Range(0, 1)] private float spikeBoundaryRight;
    [SerializeField] [Range(0, 1)] private float spikeBoundaryLeft;
    [SerializeField] [Range(0, 1)] private float scoreArea;
    [Tooltip("Decides how quickly the sound will offset. Higher value equals faster offset")]
    //[SerializeField] [Range(1, 500)] private float offsetModifier;
    [SerializeField] [Range(0, 100)] private float minimumScore;
    [Tooltip("Decides how strong the wind is. Higher value equals stronger wind")]
    [SerializeField] [Range(0, 0.1f)] private float windSpeed;
    [SerializeField] private float randomWindDuration;
    [SerializeField] private float randomWindSpawnLowerBound;
    [SerializeField] private float randomWindSpawnUpperBound;
    private float scoreAreaOffset = 0;
    [SerializeField] GameObject scoreAreaIndicatorPrefab;
    [SerializeField] private GameObject tiltIndicatorPrefab;
    private List<GameObject> scoreAreaIndicatorInstances = new List<GameObject>();
    [SerializeField] private bool useRandomWind;
    [SerializeField] private WindStruct[] winds;
    Coroutine lastMovementCoroutine;
    Coroutine lastWindCoroutine;
    private bool setPan = false;
    int index = 0;
    bool animationDone;
    private float punishment;
    private float soundPos = 0;
    private float RNGWindSpawner;
    private const int startingScore = 100;

    float permanentPositionalOffset = 0.2f;
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
            else
                ResetConditions();
        }
        else if (inSegment) //If player leaves the segment area
        {
            ExitSegment();
        }
    }

    public override void EnterSegment()
    {
        //This function gets called upon when entering the segment
        animationDone = true;
        inSegment = true;
        index = 0;
        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
        punishment = ((float)(100000 - minimumScore) / (duration.stop - duration.start));
        if (gameManager.recording)
            corruptionClearedPercent = 0;
        tiltIndicatorInstance = Instantiate(tiltIndicatorPrefab, transform);
        permanentPositionalOffset = tiltIndicatorInstance.transform.GetChild(0).localPosition.x;
        scoreAreaIndicatorInstances.Clear();
        scoreAreaIndicatorInstances.Add(Instantiate(scoreAreaIndicatorPrefab, transform));
        scoreAreaIndicatorInstances.Add(Instantiate(scoreAreaIndicatorPrefab, transform));
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

        base.ExitSegment();
        ResetConditions();
    }

    void MoveScoreArea()
    {
        lastMovementCoroutine = StartCoroutine(MoveOverBeats(scoreAreaOffset, new Vector3(nodes[index].position, 0), nodes[index].beats));
    }

    void RecordSegment()
    {

        if (animationDone && index < nodes.Count)
            MoveScoreArea();

        for(int i = 0; i < scoreAreaIndicatorInstances.Count; i++)//Temp bullshit
        {
            if (i == 0)
                scoreAreaIndicatorInstances[0].transform.localPosition = new Vector3(scoreAreaOffset*2 - scoreArea + permanentPositionalOffset, 0, -3);
            else
                scoreAreaIndicatorInstances[1].transform.localPosition = new Vector3(scoreAreaOffset*2 + scoreArea + permanentPositionalOffset, 0, -3);
        }

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

        //Keyboard functions
        if (Input.GetKey("left"))
        {
            x = -1;
        }
        else if (Input.GetKey("right"))
        {
            x = 1;
        }

        //Move sound and indicator based on x value, x is either decided by keyboard or accelerometer
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
        if (soundPos < -scoreArea + scoreAreaOffset || soundPos > scoreArea + scoreAreaOffset)
        {
            currentScore -= punishment * Time.deltaTime;
        }

        //Lose minigame if outside of boundaries
        if (soundPos <= -spikeBoundaryLeft || soundPos >= spikeBoundaryRight)
        {
            currentScore = 0;
            gameManager.recording = false;
            ExitSegment();
			audioManager.PlaySegmentClear (2f);
        }

        //Random wind handler
        RNGWindSpawner -= Time.deltaTime;
        if (RNGWindSpawner <= 0 && useRandomWind == true)
        {
            float randomDirection = Random.Range(0f, 1f);
            if(randomDirection < 0.5f)
            {
                lastWindCoroutine = StartCoroutine(WindEffect(windSpeed, false, randomWindDuration));
            }
            else
            {
                lastWindCoroutine = StartCoroutine(WindEffect(windSpeed, true, randomWindDuration));
            }

            RNGWindSpawner = Random.Range(randomWindSpawnLowerBound, randomWindSpawnUpperBound);
        }

        //Preset wind handler
        if(useRandomWind == false)
        {
            for (int i = 0; i < winds.Length; i++)
            {
                if (duration.start + winds[i].timeToAppear >= audioManager.GetTimeLinePosition() - 20 && duration.start + winds[i].timeToAppear <= audioManager.GetTimeLinePosition() + 20 && winds[i].hasBeenActive == false)
                {
                    if(winds[i].blowLeft == true)
                    {
                        lastWindCoroutine = StartCoroutine(WindEffect(windSpeed, true, (winds[i].duration/1000)));
                    }
                    else if (winds[i].blowLeft == false)
                    {
                        lastWindCoroutine = StartCoroutine(WindEffect(windSpeed, false, (winds[i].duration/1000)));
                    }
                    winds[i].hasBeenActive = true;
                }
            }
        }
        Transform flyingNote = tiltIndicatorInstance.transform.GetChild(0);
        flyingNote.localPosition = new Vector3(soundPos * 5 + permanentPositionalOffset, flyingNote.localPosition.y, flyingNote.localPosition.z);
    }

    public override void GradeScore()
    {
        base.GradeScore();
    }

    void ResetConditions()
    {
        if (lastWindCoroutine != null)
        {
            StopCoroutine(lastWindCoroutine);
            audioManager.windEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            audioManager.windEv.release();
        }
        if (lastMovementCoroutine != null)
            StopCoroutine(lastMovementCoroutine);

        if (scoreAreaIndicatorInstances.Count > 0)
        {
            foreach (GameObject indicator in scoreAreaIndicatorInstances)
                Destroy(indicator);
            scoreAreaIndicatorInstances.Clear();
        }
        animationDone = true;
        index = 0;
        soundPos = 0;
        innerDistortion = 0;
        scoreAreaOffset = 0;
        currentScore = 0;

        audioManager.musicChanSubGroup.setPan(0);
        Destroy(tiltIndicatorInstance);
        for (int i = 0; i < winds.Length; i++)
        {
            winds[i].hasBeenActive = false;
        }
    }

    IEnumerator WindEffect(float strength, bool blowLeft, float time)
    {
        if (blowLeft)
			audioManager.PlayWind (1f);
		else if (!blowLeft)
			audioManager.PlayWind (-1f);

        bool moving = false;
        if (!moving)
        {                     // Do nothing if already moving
            moving = true;                 // Set flag to true
            float t = 1.0f;
            while (t >= 0.0f)
            {
                t -= Time.deltaTime / time; // Sweeps from 0 to 1 in time seconds
                Transform wind = tiltIndicatorInstance.transform.GetChild(1);
                if (blowLeft == true)
                {
                    soundPos -= strength;
                    wind.transform.eulerAngles = new Vector3(180, 0, 180);
                    wind.gameObject.SetActive(true);
                    if(wind.localPosition.x < 0)
                        wind.localPosition = new Vector3(-wind.localPosition.x, wind.localPosition.y, wind.localPosition.z);
                }
                else if(blowLeft == false)
                {
                    soundPos += strength;
                    wind.transform.eulerAngles = new Vector3(0, 0, 0);
                    wind.gameObject.SetActive(true);
                    if (wind.localPosition.x > 0)
                        wind.localPosition = new Vector3(-wind.localPosition.x, wind.localPosition.y, wind.localPosition.z);
                }

                yield return new WaitForEndOfFrame();
            }
            tiltIndicatorInstance.transform.GetChild(1).gameObject.SetActive(false);
        }
        audioManager.windEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        audioManager.windEv.release();
    }
    public IEnumerator MoveOverBeats(float objectToMove, Vector3 end, int beats)
    {
        Vector3 objToMove = new Vector3(objectToMove, 0);
        float seconds = beats * gameManager.overallCorruption.bpmInMs/1000;
        if (animationDone == true)
        {
            animationDone = false;
            float elapsedTime = 0;
            Vector3 startingPos = new Vector3(objectToMove,0);
            while (elapsedTime < seconds)
            {
                objToMove = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
                scoreAreaOffset = objToMove.x;
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            objectToMove = end.x;
            index++;
            animationDone = true;
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

[System.Serializable]
public struct ScoreAreaNode
{
    public int beats;
    [Range (-1, 1)]public float position;

}