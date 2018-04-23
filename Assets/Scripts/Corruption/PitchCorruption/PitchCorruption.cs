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
            if (animationDone && index < nodes.Count)
            {
                MovePitchObject();
            }
            else if (index == nodes.Count)
            {
                ExitSegment();
                Destroy(gameObject);
            }
        }
        
        if (pitchSlider.value <= (transform.localPosition.y * 10) + mercyRange && pitchSlider.value >= (transform.localPosition.y * 10) - mercyRange)
            audioPitch.SetPitch(0);
        else
            audioPitch.SetPitch(pitchSlider.value - (transform.localPosition.y * 10));

        audioPitch.SetPitch(100);
    }

    void MovePitchObject()
    {
        StartCoroutine(MoveOverSeconds(gameObject, nodes[index].position, nodes[index].seconds));
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
                transform.localPosition = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.localPosition = end;
            index++;
            animationDone = true;
        }
    }
}
