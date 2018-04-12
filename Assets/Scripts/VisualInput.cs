using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualInput : MonoBehaviour {

    [Header("Particle Effects")]
    [SerializeField] private GameObject trailParticleEffect;
    [SerializeField] private GameObject touchParticleEffect;
    [Tooltip("Minimum allowed distance for a tap to trigger instead of movement.")]
    [SerializeField] private float minAllowedDistForTap = 0.25f;
    [SerializeField] private float distanceFromCamera = 5;
    [Header("Trail effect")]
    [SerializeField] private float speed = 25;
    [Header("Tap effect")]

    



    ///
    [Tooltip("Amount of times to emit effect")]
    [SerializeField] private int emitCount = 1;

    private Vector3 origPosition = Vector3.zero;

    // Temporary
    ////
    [Header("For testing purposes, activate before starting")]
    [SerializeField] private bool workWithMouseInput = false;
    ////


    void Start ()
    {
        // Temporary
        ////
        if (workWithMouseInput)
            trailParticleEffect.SetActive(true);
        ////
    }



    void Update ()
    {
        // Temporary
        ////
        if (workWithMouseInput)
        FollowMouse();
        if (!workWithMouseInput)
        ////
        FollowTouch();
    }

    void FollowTouch()
    {
        if (Input.touchCount > 0)                       // Do we have input?
        {
            Touch myTouch = Input.touches[0];

            Vector3 touchPosition = myTouch.position;
            touchPosition.z = distanceFromCamera;



            if (myTouch.phase == TouchPhase.Began)
            {
                origPosition = transform.position;      // Store position where finger started
                transform.position = Camera.main.ScreenToWorldPoint(touchPosition);
                trailParticleEffect.SetActive(true);
            }

            if (myTouch.phase == TouchPhase.Moved)
            { 
            Vector3 touchScreenToWorld = Camera.main.ScreenToWorldPoint(touchPosition);
            Vector3 position = Vector3.Lerp(transform.position, touchScreenToWorld, 1 - Mathf.Exp(-speed * Time.deltaTime));
            transform.position = position;
            }

            if (myTouch.phase == TouchPhase.Ended && Vector3.Distance(origPosition, transform.position) < minAllowedDistForTap) {
                // Ripple effect!
                // touchParticleEffect.SetActive(true);

                touchParticleEffect.GetComponent<ParticleSystem>().Play();
                // DO MORE STAFFZ MAYBE?!?!?!?!?
                //touchParticleEffect.GetComponent<ParticleSystem>().Emit(emitCount);

                //touchParticleEffect.SetActive(false);
                //trailParticleEffect.SetActive(false);
            }

            if (myTouch.phase == TouchPhase.Ended)
                trailParticleEffect.SetActive(false);
        }
    }

    // Temporary
    ////
    void FollowMouse() 
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = distanceFromCamera;

        Vector3 mouseScreenToWorld = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector3 position = Vector3.Lerp(transform.position, mouseScreenToWorld, 1 - Mathf.Exp(-speed * Time.deltaTime));

        transform.position = position;


        // DO MORE STAFFZ MAYBE?!?!?!?!?
        if (Input.GetMouseButtonDown(0))
        {
            touchParticleEffect.GetComponent<ParticleSystem>().Play();
            //touchParticleEffect.GetComponent<ParticleSystem>().Emit(emitCount);
            print("Emit shit");
        }
    }
    ////

}
