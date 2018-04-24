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
    [HideInInspector] const float startingScore = 100;

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
    

    // Use this for initialization
    void Start () {
        audioPitch = GameManager.Instance.audioPitch;
        audioManager = GameManager.Instance.audioManager;
    }
	
	// Update is called once per frame
	void Update () {
        if (audioManager.GetTimeLinePosition() >= duration.start &&
            audioManager.GetTimeLinePosition() < duration.stop) //If player is inside a corrupted area
        {
            if (!inSegment)
            {
                EnterSegment();
            }
            if (animationDone && index < nodes.Count)
            {
                MovePitchObject();
            }
            if (pitchSlider.value <= (pitchIndicatorInstance.transform.localPosition.y * 10) + mercyRange && pitchSlider.value >= (pitchIndicatorInstance.transform.localPosition.y * 10) - mercyRange)
                audioPitch.SetPitch(0, PitchType.All);
            else
            {
                audioPitch.SetPitch(pitchSlider.value - (pitchIndicatorInstance.transform.localPosition.y * 10), PitchType.All);
                score -= (punishment * Time.deltaTime);
            }

        }
        else if (inSegment) //If player leaves corrupted area
        {
            ExitSegment();
        }
    }

    public override void EnterSegment()
    {
        score = startingScore;
        totalSeconds = 0;
        foreach(PitchNode node in nodes)
        {
            totalSeconds += node.seconds;
        }
        punishment = score / totalSeconds;
        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
        Debug.Log(innerDistortion);
        pitchIndicatorInstance = Instantiate(pitchIndicator, gameObject.transform);
        base.EnterSegment();
    }

    public override void ExitSegment()
    {
        corruptionClearedPercent = score;
        innerDistortion = 0;
        StopCoroutine(lastCoroutine);
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
