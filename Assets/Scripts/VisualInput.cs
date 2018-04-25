using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualInput : MonoBehaviour
{

    [TextArea]
    public string Notes = "This game object can be anywhere in the scene. The trail effect needs to be attached to the game object but the touch particle should be a prefab.";

    [Header("Particle Effects")]
    [SerializeField] private GameObject trailParticleEffect;
    [SerializeField] private GameObject touchParticleEffectPrefab;
    [SerializeField] private float distanceFromCamera = 5;
    [Header("Trail effect")]
    [SerializeField] private float speed = 25;
    [Header("Tap effect")]
    [Header("The DrumCorruption prefab also needs extra information for this to work")]
    [SerializeField] private bool useDrumCorruptionToMatchTiming;
    [Header("Color of tap effect depending on timing of drumbeat")]
    [SerializeField] private Color originalRippleColor;
    [SerializeField] private Color okayRippleColor;
    [SerializeField] private Color perfectRippleColor;




    [SerializeField] private Color changedRippleColor;
    


    private Vector3 origPosition = Vector3.zero;

    private DrumMechanic drumMechanic;      // Fetch from GameManager later


    // Temporary
    ////
    [Header("For testing purposes, activate before starting")]
    [SerializeField] private bool workWithMouseInput = false;
    ////


    void Start()
    {
        // Temporary
        ////
        if (workWithMouseInput)
            trailParticleEffect.SetActive(true);
        ////

        if (GameManager.Instance.drumMechanic)
            drumMechanic = GameManager.Instance.drumMechanic.GetComponent<DrumMechanic>();
    }



    void Update()
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
                origPosition = transform.position;      // Store position where object was located
                transform.position = Camera.main.ScreenToWorldPoint(touchPosition);     // Set position to where touch was detected
                trailParticleEffect.SetActive(true);
            }

            if (myTouch.phase == TouchPhase.Moved)
            {
                Vector3 touchScreenToWorld = Camera.main.ScreenToWorldPoint(touchPosition);
                Vector3 position = Vector3.Lerp(transform.position, touchScreenToWorld, 1 - Mathf.Exp(-speed * Time.deltaTime));
                transform.position = position;             // Move object with lerp to position of touch
                if (origPosition == -Vector3.one)      // Original position will be a negative Vector3.one if touch has ended, so swap to positivie Vector3.one for tap-on effect check
                {
                    origPosition = Vector3.one;
                }
            }

            if (myTouch.phase == TouchPhase.Ended && (origPosition == -Vector3.one || origPosition == Vector3.one))     // If original position is outside of screen or specifically a Vector3.one
            {                                                                                                               // we've only tapped the screen
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(touchPosition);                                       // Save position to instantiate touch effect in world space

                foreach (Transform effect in touchParticleEffectPrefab.transform)                                             // Check for multiple particle effects and run them all once
                {
                    GameObject temp = Instantiate(effect.gameObject, worldPos, Quaternion.Euler(0, 0, 0));
                    temp.GetComponent<ParticleSystem>().Play();
                    Destroy(temp, temp.GetComponent<ParticleSystem>().main.duration);
                }
            }

            if (myTouch.phase == TouchPhase.Ended)
            {
                trailParticleEffect.SetActive(false);
                transform.position = -Vector3.one;
            }
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

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);

            foreach (Transform effect in touchParticleEffectPrefab.transform)
            {
                GameObject temp = Instantiate(effect.gameObject, worldPos, Quaternion.Euler(0, 0, 0));
                
                
                if (useDrumCorruptionToMatchTiming && drumMechanic.recording)
                {
                    ParticleSystem.MainModule tempa = temp.GetComponent<ParticleSystem>().main;
                    tempa.startColor = CatchColorChange();
                }
                

                /*
                if (drumMechanic != null && drumCorruption != null && drumMechanic.recording)
                {
                    drumCorruption = GameManager.Instance.drumMechanic.GetComponentInChildren<DrumCorruption>();
                    print(drumMechanic);
                    ParticleSystem.MainModule tempa = temp.GetComponent<ParticleSystem>().main;
                    Timing timing = drumCorruption.CheckTiming();
                    if (timing == Timing.perfect)
                        print("JIA)#I=?WD=");
                        //tempa.startColor = Color.green;
                    if (drumCorruption.CheckTiming() == Timing.okay)
                        tempa.startColor = Color.red;
                    else
                        tempa.startColor = originalRippleColor;
                }
                */

                temp.GetComponent<ParticleSystem>().Play();
                Destroy(temp, temp.GetComponent<ParticleSystem>().main.duration);
            }
        }
    }
    ////

    public void ChangeColorOnTiming(Timing timing)
    {
        if (timing == Timing.perfect)
            changedRippleColor = perfectRippleColor;
        if (timing == Timing.okay)
            changedRippleColor = okayRippleColor;
        else
            changedRippleColor = originalRippleColor;            
    }

    private Color CatchColorChange()
    {
        return changedRippleColor;
    }

}
