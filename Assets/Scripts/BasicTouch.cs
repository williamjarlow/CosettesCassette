using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTouch : MonoBehaviour {

    [SerializeField]
    private float inputLengthMultiplier;
    [HideInInspector]
    public float touchDiff;
    private float origin;

	// Use this for initialization
	void Start ()
    {
	    	
	}
	
	// Update is called once per frame
	void Update ()
    {

            if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                origin = Input.GetTouch(0).position.x;
            }

            else if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                float endPoint = Input.GetTouch(0).position.x;
                touchDiff = Mathf.Clamp((endPoint - origin) * inputLengthMultiplier, -50, 50);
            Debug.Log(touchDiff);

            }
    }
}
