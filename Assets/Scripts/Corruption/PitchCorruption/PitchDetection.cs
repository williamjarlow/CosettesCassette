using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PitchNode
{
    public Vector3 position = new Vector3();
    public float seconds = 0;
}

public class PitchDetection : MonoBehaviour {
    [SerializeField]
    LayerMask layerMask;

    public List<PitchNode> nodes = new List<PitchNode>();

    [SerializeField] Vector2 rGoalRange = new Vector2(-2, 2);
    [SerializeField] Vector2 rTravelTimeRange = new Vector2(1, 3);
    int nodeAmount;

    int index = 0;

    bool animationDone = true;

    [SerializeField] bool randomizeNodes;

    // Use this for initialization
    void Start () {
        nodeAmount = nodes.Count;
        if (randomizeNodes)
        {
            nodes.Clear();
            Randomize();
        }
        MovePitchObject();
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

    void Randomize()
    {
        for (int i = 0; i < nodeAmount; i++)
        {
            nodes.Add(new PitchNode());
            nodes[i].seconds = Random.Range(rTravelTimeRange.x, rTravelTimeRange.y);
            nodes[i].position = new Vector3(gameObject.transform.localPosition.x, Random.Range(rGoalRange.x, rGoalRange.y), gameObject.transform.localPosition.z);
        }
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
