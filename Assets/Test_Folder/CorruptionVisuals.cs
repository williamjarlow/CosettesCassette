using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CorruptionVisuals : MonoBehaviour {

    public List<Rect> rectList;
    public List<Texture> imageList;


	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    { 
        GUI.Box(rectList[0], imageList[0]);

    }
}
