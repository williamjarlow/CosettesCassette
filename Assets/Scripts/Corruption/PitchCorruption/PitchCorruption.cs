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
    [HideInInspector] public Slider pitchSlider;
    [HideInInspector] public List<PitchNode> nodes = new List<PitchNode>();
    AudioPitch audioPitch;
    int index = 0;
    bool animationDone = true;

    // Use this for initialization
    void Start () {
        audioPitch = GameManager.Instance.audioPitch;
    }
	
	// Update is called once per frame
	void Update () {
        if (animationDone && index < nodes.Count)
        {
            MovePitchObject();
        }

       /* if (!DetectRaycastCollision() && animationDone == false)
        {
        }*/
    }

    /*bool DetectRaycastCollision()
    {
        Vector2 pos = Input.mousePosition; // Mouse position
        RaycastHit hit;
        Camera _cam = Camera.main; // Camera to use for raycasting
        Ray ray = _cam.ScreenPointToRay(pos);
        Physics.Raycast(_cam.transform.position, ray.direction, out hit, 10000.0f, layerMask);
        if (hit.collider)
        {
            return true;
        }
        return false;
    }*/

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
