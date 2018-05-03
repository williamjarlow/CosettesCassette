using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PitchNode
{
    public Vector3 position = new Vector3();
    public float seconds = 0;
}

public class PitchCorruption : CorruptionBaseClass {
    [Header("Pitch nodes")]
    public List<PitchNode> nodes = new List<PitchNode>();

    public Slider pitchSlider;
    public float mercyRange;

    public Vector2 rGoalRange = new Vector2(-2, 2);
    public Vector2 rTravelTimeRange = new Vector2(1, 3);

    [Header("Random generation of pitch nodes")]
    public bool randomizeNodes;

    const float startingScore = 100;
    const int pitchIndicatorMax = 2; //This constant represents the maximum positional value that the pitch indicator can have.

    float savedTime;

    float totalTime; //The total amount of time measured in seconds
    float hitTime; //The amount of time the player was in the zone in seconds

    float totalSeconds;
    float score;
    Vector3 startingPosition;

    [SerializeField] GameObject pitchIndicator;
    GameObject pitchIndicatorInstance;
    Coroutine lastCoroutine;
    AudioManager audioManager;
    AudioPitch audioPitch;
    int index = 0;
    bool animationDone = true;

    private OverallCorruption overallCorruption;

    // Use this for initialization
    void Start()
    {
        audioPitch = GameManager.Instance.audioPitch;
        audioManager = GameManager.Instance.audioManager;
        overallCorruption = GameManager.Instance.overallCorruption;
        // Set the pitch segments to the recording type PITCH 
        overallCorruption.durations[segmentID].recordingType = Duration.RecordingType.PITCH;

        if (randomizeNodes) //This code randomizes the nodes rather than use the nodes specified in the inspector.
                            //Useful for testing.
        {
            int nodesCount = nodes.Count;
            nodes.Clear();
            for (int i = 0; i < nodesCount; i++)
            {
                nodes.Add(new PitchNode());
                if (i == 0)
                {
                    nodes[i].seconds = 1;
                    nodes[i].position = new Vector3(gameObject.transform.localPosition.x, 0, gameObject.transform.localPosition.z);
                }
                else
                {
                    nodes[i].seconds = Random.Range(rTravelTimeRange.x, rTravelTimeRange.y);
                    nodes[i].position = new Vector3(gameObject.transform.localPosition.x, Random.Range(rGoalRange.x, rGoalRange.y), gameObject.transform.localPosition.z);
                }
            }
        }
        duration = overallCorruption.durations[segmentID];
    }
	
	// Update is called once per frame
	void Update () {

        if (audioManager.GetTimeLinePosition() > duration.start &&
            audioManager.GetTimeLinePosition() < duration.stop) //If player is inside a corrupted area
        {
            if (!inSegment) //inSegment is a bool that toggles when you enter and exit a segment.
            {
                EnterSegment();
            }
            RecordPitch();
        }
        else if (inSegment) //If player leaves corrupted area
        {
            ExitSegment();
        }
    }

    public override void EnterSegment()
    {
        hitTime = 0;
        float totalNodeTime = 0;
        foreach(PitchNode node in nodes)
        {
            totalNodeTime += node.seconds;
        }

        if ((duration.stop - duration.start) / 1000 < totalNodeTime)
            totalTime = (duration.stop - duration.start) / 1000;
        else
            totalTime = totalNodeTime;

        index = 0;
        animationDone = true;
        score = startingScore; //Score starts at 100 and decreases when the player makes mistakes.
        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
        pitchIndicatorInstance = Instantiate(pitchIndicator, gameObject.transform); //Create an instance of the object that the player needs to follow.
        base.EnterSegment();
    }

    public override void ExitSegment()
    {
        if (totalTime != 0)
        {
            score = (hitTime / totalTime) * 100;
            if (score > 100)
                score = 100;
        }
        else
            score = 0;

        corruptionClearedPercent = score;
        savedTime = hitTime;
        innerDistortion = 0;
        index = 0;
        StopCoroutine(lastCoroutine); //This is neccessary in order to ensure that nothing breaks if the corruption gets ended early.
        Destroy(pitchIndicatorInstance);
        audioPitch.SetPitch(0, PitchType.All);
        base.ExitSegment();
    }

    void RecordPitch()
    {
        if (index < nodes.Count) //while there are more nodes to visit in the node list
        {
            if (animationDone) //if the current animation is done, start a new one.
            {
                MovePitchObject();
            }
            if (pitchSlider.value <= (pitchIndicatorInstance.transform.localPosition.y * (pitchSlider.maxValue / pitchIndicatorMax)) + mercyRange &&
                pitchSlider.value >= (pitchIndicatorInstance.transform.localPosition.y * (pitchSlider.maxValue / pitchIndicatorMax) - mercyRange))
            {
                audioPitch.SetPitch(0, PitchType.All); //If the player is within acceptable margin, let the pitch be normal.
                if (GameManager.Instance.recording)
                    hitTime += Time.deltaTime;
                else
                    hitTime = savedTime;
            }
            else
            {
                audioPitch.SetPitch(pitchSlider.value - (pitchIndicatorInstance.transform.localPosition.y *
                (pitchSlider.maxValue / pitchIndicatorMax)), PitchType.All);
                if (!GameManager.Instance.recording)
                    hitTime = savedTime; //If player isn't recording, don't change score.
            }
        }
    }

    void MovePitchObject()
    {
        lastCoroutine = StartCoroutine(MoveOverSeconds(pitchIndicatorInstance, nodes[index].position, nodes[index].seconds));
    }

    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        if (animationDone == true)
        {
            animationDone = false;
            float elapsedTime = 0;
            Vector3 startingPos = objectToMove.transform.localPosition;
            while (elapsedTime < seconds)
            {
                objectToMove.transform.localPosition = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            objectToMove.transform.localPosition = end;
            index++;
            animationDone = true;
        }
    }
}
