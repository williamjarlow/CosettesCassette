using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchControls : MonoBehaviour {

    public GameObject particle;
	void Start ()
    {
        
	}

	void Update ()
    {

        if (Input.touchCount > 0)       // Registered a touch
        {
            Touch myTouch = Input.touches[0];   // Grab first touch

            if (myTouch.phase == TouchPhase.Began)
            {
                // Construct a ray from the current touch coordinates
               // Ray ray = Camera.main.ScreenPointToRay(myTouch.position);
                // Create a particle if hit
               // if (Physics.Raycast(ray))
                    Instantiate(particle, myTouch.position, transform.rotation, gameObject.transform);
            }
        }
    }
}
