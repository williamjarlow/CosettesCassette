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

    Vector2 goalRange = new Vector2(-1, 3);
    Vector2 speedRange = new Vector2(1, 3);
    const int nodeAmount = 100;

    int index = 0;

    bool animationDone = true;

    [SerializeField] bool randomizeNodes;

    // Use this for initialization
    void Start () {
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

        if (!DetectRaycastCollision() && animationDone == false)
        {
        }
    }

    bool DetectRaycastCollision()
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
    }

    void Randomize()
    {
        for (int i = 0; i < nodeAmount; i++)
        {
            nodes.Add(new PitchNode());
            nodes[i].seconds = Random.Range(speedRange.x, speedRange.y);
            nodes[i].position = new Vector3(gameObject.transform.position.x, Random.Range(goalRange.x, goalRange.y), gameObject.transform.position.z);
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
            Vector3 startingPos = objectToMove.transform.position;
            while (elapsedTime < seconds)
            {
                transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.position = end;
            index++;
            animationDone = true;
        }
    }
}
