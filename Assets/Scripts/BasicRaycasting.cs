using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRaycasting : MonoBehaviour {

    public LevelSelect levelSelect;

	// Use this for initialization
	void Start () {
        Debug.Assert(levelSelect != null, "Cassette base needs to be added as level select");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && !levelSelect.pauseScreen)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Cassette")
                {
                    hit.transform.SendMessage("SwitchScene");
                }
            }
        }
    }
}
