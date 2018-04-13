using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour {

    [SerializeField] private float speed;
    [SerializeField] private GameObject stopper;
    [SerializeField] private float endPos;

    // Use this for initialization
    void Start ()
    {

        endPos = stopper.transform.position.y;

        		
	}
	
	// Update is called once per frame
	void Update ()
    {

        
        if (transform.position.y < endPos)
        {
            transform.Translate(Vector3.up * speed);
          
        }
        else
        {
            transform.Translate(Vector3.up * 0);
            
        }
    }
}
