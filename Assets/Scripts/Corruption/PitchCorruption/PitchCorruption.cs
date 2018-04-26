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
    [HideInInspector] public float mercyRange;
    [HideInInspector] public Slider pitchSlider;
    [HideInInspector] public List<PitchNode> nodes = new List<PitchNode>();

    const float startingScore = 100;

    float punishment;
    float totalSeconds;
    float score;

    [SerializeField] GameObject pitchIndicator;
    GameObject pitchIndicatorInstance;
    Coroutine lastCoroutine;
    AudioManager audioManager;
    AudioPitch audioPitch;
    int index = 0;
    bool animationDone = true;

    private OverallCorruption overallCorruption;

    // Use this for initialization
    void Start () {
        audioPitch = GameManager.Instance.audioPitch;
        audioManager = GameManager.Instance.audioManager;
        overallCorruption = GameManager.Instance.overallCorruption;

        // Set the pitch segments to the recording type PITCH 
        overallCorruption.durations[segmentID].recordingType = Duration.RecordingType.PITCH;
    }
	
	// Update is called once per frame
	void Update () {

        if ((audioManager.GetTimeLinePosition() >= duration.start &&
            audioManager.GetTimeLinePosition() < duration.stop ) && !cleared) //If player is inside a corrupted area
        {
            if (!inSegment) //inSegment is a bool that toggles when you enter and exit a segment.
            {
                EnterSegment();
            }
            if (index < nodes.Count) //while there are more nodes to visit in the node list
            {
                if (animationDone) //if the current animation is done, start a new one.
                {
                    MovePitchObject();
                }
                if (pitchSlider.value <= (pitchIndicatorInstance.transform.localPosition.y * 10) + mercyRange && pitchSlider.value >= (pitchIndicatorInstance.transform.localPosition.y * 10) - mercyRange)
                    audioPitch.SetPitch(0, PitchType.All); //If the player is within acceptable margin, let the pitch be normal.
                else
                {
                    audioPitch.SetPitch(pitchSlider.value - (pitchIndicatorInstance.transform.localPosition.y * 10), PitchType.All);
                    score -= (punishment * Time.deltaTime); //If the player isn't within acceptable margin, mess the pitch up accordingly.
                }
            }

        }
        else if (inSegment) //If player leaves corrupted area
        {
            ExitSegment();
        }
    }

    public override void EnterSegment()
    {
        score = startingScore; //Score starts at 100 and decreases when the player messes up.
        totalSeconds = 0;   //Amount of time that the entire corruption should take. This is what is used to calculate how punished the player will be.
        foreach(PitchNode node in nodes)
        {
            totalSeconds += node.seconds;
        }
        punishment = score / totalSeconds;
        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
        pitchIndicatorInstance = Instantiate(pitchIndicator, gameObject.transform); //Create an instance of the object that the player needs to follow.
        base.EnterSegment();
    }

    public override void ExitSegment()
    {
        if (score < 0)
            score = 0;
        corruptionClearedPercent = score;
        innerDistortion = 0;
        index = 0;
        StopCoroutine(lastCoroutine); //This is neccessary in order to ensure that nothing breaks if the corruption gets ended early.
        Destroy(pitchIndicatorInstance);
        audioPitch.SetPitch(0, PitchType.All);
        base.ExitSegment();
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
