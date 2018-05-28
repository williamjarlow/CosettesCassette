using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PitchNode
{
    [Range (-2, 2)]
    public float position = 0;
    public float seconds = 0;
}

public class PitchCorruption : CorruptionBaseClass {
    [Header("Pitch nodes")]
    public List<PitchNode> nodes = new List<PitchNode>();

    [SerializeField]
    bool curvify;
    [SerializeField]
    float temporaryTestValue;

    [SerializeField] [Tooltip("What instrument the pitch shifting should affect.")]
    PitchType pitchType;

    public float lineSpeed;

    LineRenderer lineRenderer;
    float timeSinceStart;

    private Slider pitchSlider;
    public float mercyRange;

    [Header ("Randomness variables")] [Tooltip ("Set the values of rGoalRange between -2 and 2. The x value has to be lower than the y value.")]
    public Vector2 rGoalRange = new Vector2(-2, 2);
    [Tooltip("Set the values of rTravelTimeRange between 1 and 3. The x value has to be lower than the y value.")]
    public Vector2 rTravelTimeRange = new Vector2(1, 3);

    [Header("Random generation of pitch nodes")]
    public bool randomizeNodes;

    [SerializeField] [Range(0, 5)]
    float leadUp;
    [SerializeField]
    [Range(0.1f, 1)]
    float ruggedness;

    bool inCorruption = false;

    const float startingScore = 100;
    const int pitchIndicatorMax = 2; //This constant represents the maximum positional value that the pitch indicator can have.

    float totalTime; //The total amount of time measured in seconds
    float hitTime; //The amount of time the player was in the zone in seconds

    Vector3 startingPosition;

    [SerializeField] GameObject pitchIndicator;
    [SerializeField] GameObject pitchPixelParticlePrefab;
    GameObject pitchPixelParticleInstance;
    GameObject pitchIndicatorInstance;
    Coroutine lastCoroutine;
    AudioManager audioManager;
    AudioPitch audioPitch;
    int index = 0;
    bool animationDone = true;

    List<Vector3> linePositions = new List<Vector3>();

    private OverallCorruption overallCorruption;

    // Use this for initialization
    void Start()
    {
        pitchSlider = gameManager.pitchSlider.GetComponent<Slider>();
        pitchSlider.gameObject.SetActive(false);
        lineRenderer = GetComponent<LineRenderer>();
        audioPitch = gameManager.audioPitch;
        audioManager = gameManager.audioManager;
        overallCorruption = gameManager.overallCorruption;
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
                    nodes[i].position = 0;
                }
                else
                {
                    nodes[i].seconds = Random.Range(rTravelTimeRange.x, rTravelTimeRange.y);
                    nodes[i].position = Random.Range(rGoalRange.x, rGoalRange.y);
                }
            }
        }
        duration = overallCorruption.durations[segmentID];
        Load();
    }
	
	// Update is called once per frame
	void Update () {
    
        if(audioManager.GetTimeLinePosition() > duration.start && audioManager.GetTimeLinePosition() < duration.stop) //If player is inside a corrupted area
        {
            if (gameManager.recording)
            {
                timeSinceStart += Time.deltaTime;
                if (!inSegment) //inSegment is a bool that toggles when you enter and exit a segment.
                {
                    EnterSegment();
                }
                if (timeSinceStart >= leadUp)
                    RecordPitch();
                MoveLine();
            }
            else
            {
                if (!cleared)
                {
                    if (!inSegment)
                    {
                        EnterSegment();
                        inSegment = true;
                        hitTime = 0;
                        timeSinceStart = 0;
                        index = 0;
                    }
                    pitchSlider.gameObject.SetActive(false);
                    DestroyLine();
                    RecordPitch();
                    //ResetConditions();
                }
            }
        }
        else if (inSegment) //If player leaves corrupted area
        {
            ExitSegment();
        }
    }

    void ResetConditions()
    {
        pitchSlider.gameObject.SetActive(false);
        audioPitch.TogglePitch(pitchType, 0f);
        GradeScore();
        timeSinceStart = 0;
        innerDistortion = 0;
        index = 0;
        if (lastCoroutine != null)
            StopCoroutine(lastCoroutine); //This is neccessary in order to ensure that nothing breaks if the corruption gets ended early.
        Destroy(pitchIndicatorInstance);
        audioPitch.SetPitch(0, pitchType);
        DestroyLine();
    }

    void GenerateLine()
    {
        lineRenderer.positionCount = nodes.Count + 1;

        Vector3 newPosition;
        for (int i = 0; i <= nodes.Count; i++)
        {
            if (i == 0)
                newPosition = new Vector3(0, 0);
            else
                newPosition = new Vector3(lineRenderer.GetPosition(i - 1).x - nodes[i-1].seconds * lineSpeed, nodes[i-1].position);

            lineRenderer.SetPosition(i, newPosition); // Create the points in the line
        }

        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            newPosition = new Vector3(positions[i].x - leadUp * lineSpeed, positions[i].y, positions[i].z);
            lineRenderer.SetPosition(i, newPosition); // Create the points in the line
        }
        lineRenderer.GetPositions(positions);

        //Curved Lines
        if (curvify)
        {
            linePositions.Clear();
            linePositions.AddRange(positions);

            List<Vector3> newList = new List<Vector3>();
            for (int i = 0; i < linePositions.Count - 2; i++)
            {
                //Math.
                float m1 = (Mathf.Abs(linePositions[i + 1].x) - Mathf.Abs(linePositions[i].x)) / 2;
                float m2 = (Mathf.Abs(linePositions[i + 2].x) - Mathf.Abs(linePositions[i + 1].x)) / 2;
                float xDist = m1 + m2; //Beräkna avstånd mellan x värden

                List<Vector3> temp = new List<Vector3>();
                float a = -nodes[i].position / (xDist * xDist);

                for (float j = xDist / 2; j > -xDist / 2; j -= ruggedness)
                {
                    temp.Add(new Vector3(j + temporaryTestValue, (a * (j + xDist / 2) * (j - xDist / 2)) * 4));
                }

                foreach (Vector3 position in temp)
                {
                    newList.Add(new Vector3(linePositions[i].x + position.x, position.y));
                }
            }
            lineRenderer.positionCount = newList.Count;
            lineRenderer.SetPositions(newList.ToArray());

            positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);

            nodes.Clear();
            foreach (Vector3 position in positions)
            {
                PitchNode node = new PitchNode();
                node.position = position.y;
                node.seconds = totalTime / positions.Length;
                nodes.Add(node);
            }
        }

    }
    void MoveLine()
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            positions[i] = new Vector3(positions[i].x + Time.deltaTime * lineSpeed, positions[i].y, positions[i].z);
        }
        lineRenderer.SetPositions(positions);
    }

    void DestroyLine()
    {
        lineRenderer.positionCount = 0;
    }

    public override void EnterSegment()
    {
        pitchSlider.gameObject.SetActive(true);
        pitchSlider.value = 0;
        hitTime = 0;
        float totalNodeTime = 0;

		audioPitch.TogglePitch (pitchType, 1f);

        foreach(PitchNode node in nodes)
        {
            totalNodeTime += node.seconds;
        }


        if ((duration.stop - duration.start) / 1000 < totalNodeTime) //Ensures that designers can't fuck up.
            totalTime = (duration.stop - duration.start) / 1000;     //Check to see which one of total node time and segment 
        else                                                         //duration is shortest. Use the shorter one.
            totalTime = totalNodeTime;

        index = 0;
        animationDone = true;
        currentScore = startingScore; //Score starts at 100 and decreases when the player makes mistakes.
        innerDistortion = maxDistortion * (1 - (corruptionClearedPercent / 100));
        pitchIndicatorInstance = Instantiate(pitchIndicator, gameObject.transform); //Create an instance of the object that the player needs to follow.
        GenerateLine();
        base.EnterSegment();
    }

    public override void ExitSegment()
    {
        ResetConditions();
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
                audioPitch.SetPitch(0, pitchType); //If the player is within acceptable margin, let the pitch be normal.
                if(gameManager.recording)
                    hitTime += Time.deltaTime;
            }
            else
            {
                audioPitch.SetPitch(pitchSlider.value - (pitchIndicatorInstance.transform.localPosition.y *
                (pitchSlider.maxValue / pitchIndicatorMax)), pitchType);
            }
        }
    }

    void MovePitchObject()
    {
        lastCoroutine = StartCoroutine(MoveOverSeconds(pitchIndicatorInstance, new Vector3(0, nodes[index].position), nodes[index].seconds));
    }

    public override void GradeScore()
    {
        if (totalTime != 0)
            currentScore = (hitTime / totalTime) * 100;
        else
            currentScore = 0;
        base.GradeScore();
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
