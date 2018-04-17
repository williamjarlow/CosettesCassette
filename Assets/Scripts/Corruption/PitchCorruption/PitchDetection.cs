using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchDetection : MonoBehaviour {
    [SerializeField]
    LayerMask layerMask;
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        Vector2 pos = Input.mousePosition; // Mouse position
        RaycastHit hit;
        Camera _cam = Camera.main; // Camera to use for raycasting
        Ray ray = _cam.ScreenPointToRay(pos);
        Physics.Raycast(_cam.transform.position, ray.direction, out hit, 10000.0f, layerMask);
        if (hit.collider)
        {
            Debug.Log("Touching");
        }
    }

}
