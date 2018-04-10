using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualInput : MonoBehaviour {


    public GameObject particleEffect;
    public float speed = 8;
    public float distanceFromCamera = 5;


    void Start ()
    {

	}



    void Update ()
    {
        //FollowMouse();
        FollowTouch();
    }

    void FollowTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];

            Vector3 touchPosition = myTouch.position;
            touchPosition.z = distanceFromCamera;

            if (myTouch.phase == TouchPhase.Began)
            {
                transform.position = Camera.main.ScreenToWorldPoint(touchPosition);
                particleEffect.SetActive(true);
            }

            Vector3 touchScreenToWorld = Camera.main.ScreenToWorldPoint(touchPosition);

            Vector3 position = Vector3.Lerp(transform.position, touchScreenToWorld, 1 - Mathf.Exp(-speed * Time.deltaTime));

            transform.position = position;

            if (myTouch.phase == TouchPhase.Ended)
                particleEffect.SetActive(false);
        }
    }

    void FollowMouse() 
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = distanceFromCamera;

        Vector3 mouseScreenToWorld = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector3 position = Vector3.Lerp(transform.position, mouseScreenToWorld, 1 - Mathf.Exp(-speed * Time.deltaTime));

        transform.position = position;
    }

}
