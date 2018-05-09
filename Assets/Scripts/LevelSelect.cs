using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class LevelSelect : MonoBehaviour
{

    private int currentFocus;

    private int cassetteAmount;

    [SerializeField] private int rotationAmount = 50;

    public List<RaycastObject> cassettes = new List<RaycastObject>();

    private Vector3 origPosition = Vector3.zero;

    [SerializeField] private float minimumMovementForChange = 5;
    [SerializeField] private float rotationDuration;

    // Temporary
    ////
    [Header("For testing purposes, activate before starting")]
    [SerializeField] private bool workWithMouseInput = false;
    ////

    private Vector3[] startPos;
    private bool aids = false;

    void Start()
    {
        cassetteAmount = cassettes.Count;
        currentFocus = cassetteAmount;
        startPos = new Vector3[cassettes.Count];
        for(int i = 0; i < cassetteAmount; i++)
        {
            startPos[i] = cassettes[i].transform.localPosition;
        }
    }


    void Update()
    {
        TouchControls();


        // Temporary
        ////
        if (workWithMouseInput)
        MouseControls();
        ////
    }

    private void MouseControls()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            if (currentFocus >= -1 && currentFocus < cassetteAmount - 1 && aids == false)
            {
                aids = true;
                currentFocus++;
                Quaternion flatRotation = Quaternion.Euler(270, 0, 0);
                Quaternion standingRotation = Quaternion.Euler(-30, 0, 0);
                Quaternion reversedStandingRotation = Quaternion.Euler(210, 0, 0);
                StartCoroutine(MoveFromTo(new Vector3(startPos[currentFocus].x, startPos[currentFocus].y, startPos[currentFocus].z + 3), startPos[currentFocus], rotationDuration, currentFocus, flatRotation, reversedStandingRotation));

                for(int i = cassetteAmount - 1; i > 0; i--)
                {
                    if(i < currentFocus - 1)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z + 1), startPos[i], rotationDuration, i, standingRotation, standingRotation));
                    }
                    else if(i > currentFocus)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z + 1), startPos[i], rotationDuration, i, reversedStandingRotation, reversedStandingRotation));
                    }
                    else if (i == currentFocus - 1)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z + 3), startPos[i], rotationDuration, i, standingRotation, flatRotation));
                    }
                }
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            if (currentFocus > 0 && currentFocus <= cassetteAmount && aids == false)
            {
                aids = true;
                currentFocus--;
                Quaternion flatRotation = Quaternion.Euler(270, 0, 0);
                Quaternion standingRotation = Quaternion.Euler(-30, 0, 0);
                Quaternion reversedStandingRotation = Quaternion.Euler(210, 0, 0);
                StartCoroutine(MoveFromTo(new Vector3(startPos[currentFocus].x, startPos[currentFocus].y, startPos[currentFocus].z - 3), startPos[currentFocus], rotationDuration, currentFocus, flatRotation, standingRotation));

                for (int i = cassetteAmount - 1; i > 0; i--)
                {
                    if (i < currentFocus)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z - 1), startPos[i], rotationDuration, i, standingRotation, standingRotation));
                    }
                    else if (i > currentFocus+1)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z - 1), startPos[i], rotationDuration, i, reversedStandingRotation, reversedStandingRotation));
                    }
                    else if(i == currentFocus+1)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z - 3), startPos[i], rotationDuration, i, reversedStandingRotation, flatRotation));
                    }
                }
            }
        }
    }

    private void TouchControls()
    {
        if (Input.touchCount > 0)                       // Do we have input?
        {
            Touch myTouch = Input.touches[0];

            Vector3 touchPosition = myTouch.position;

            if (myTouch.phase == TouchPhase.Began)
            {
                origPosition = touchPosition;
            }
            if (myTouch.phase == TouchPhase.Moved)
            {
                touchPosition = myTouch.position;
                if ((touchPosition.y - origPosition.y) >= minimumMovementForChange)
                {
                    if (currentFocus > 0 && currentFocus <= cassetteAmount && aids == false)
                    {
                        aids = true;
                        currentFocus--;
                        Quaternion flatRotation = Quaternion.Euler(270, 0, 0);
                        Quaternion standingRotation = Quaternion.Euler(-30, 0, 0);
                        Quaternion reversedStandingRotation = Quaternion.Euler(210, 0, 0);
                        StartCoroutine(MoveFromTo(new Vector3(startPos[currentFocus].x, startPos[currentFocus].y, startPos[currentFocus].z - 3), startPos[currentFocus], rotationDuration, currentFocus, flatRotation, standingRotation));

                        for (int i = cassetteAmount - 1; i > 0; i--)
                        {
                            if (i < currentFocus)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z - 1), startPos[i], rotationDuration, i, standingRotation, standingRotation));
                            }
                            else if (i > currentFocus + 1)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z - 1), startPos[i], rotationDuration, i, reversedStandingRotation, reversedStandingRotation));
                            }
                            else if (i == currentFocus + 1)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z - 3), startPos[i], rotationDuration, i, reversedStandingRotation, flatRotation));
                            }
                        }
                    }
                }
                if ((touchPosition.y - origPosition.y) < minimumMovementForChange)
                {
                    if (currentFocus >= -1 && currentFocus < cassetteAmount - 1 && aids == false)
                    {
                        aids = true;
                        currentFocus++;
                        Quaternion flatRotation = Quaternion.Euler(270, 0, 0);
                        Quaternion standingRotation = Quaternion.Euler(-30, 0, 0);
                        Quaternion reversedStandingRotation = Quaternion.Euler(210, 0, 0);
                        StartCoroutine(MoveFromTo(new Vector3(startPos[currentFocus].x, startPos[currentFocus].y, startPos[currentFocus].z + 3), startPos[currentFocus], rotationDuration, currentFocus, flatRotation, reversedStandingRotation));

                        for (int i = cassetteAmount - 1; i > 0; i--)
                        {
                            if (i < currentFocus - 1)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z + 1), startPos[i], rotationDuration, i, standingRotation, standingRotation));
                            }
                            else if (i > currentFocus)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z + 1), startPos[i], rotationDuration, i, reversedStandingRotation, reversedStandingRotation));
                            }
                            else if (i == currentFocus - 1)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z + 3), startPos[i], rotationDuration, i, standingRotation, flatRotation));
                            }
                        }
                    }
                }
            }
            if (myTouch.phase == TouchPhase.Ended)
                touchPosition = new Vector3(Vector3.zero.x, touchPosition.y, Vector3.zero.z);
        }
    }
    IEnumerator MoveFromTo(Vector3 pointA, Vector3 pointB, float time, int chosen, Quaternion fromRot, Quaternion targetRot)
    {
        bool moving = false;
        if (!moving)
        {                     // Do nothing if already moving
            moving = true;                 // Set flag to true
            float t = 1.0f;
            while (t >= 0.0f)
            {
                t -= Time.deltaTime / time; // Sweeps from 0 to 1 in time seconds
                cassettes[chosen].transform.localPosition = Vector3.Lerp(pointA, pointB, t); // Set position proportional to t
                cassettes[chosen].transform.localRotation = Quaternion.Slerp(fromRot, targetRot, t);
                yield return new WaitForEndOfFrame();
            }
            moving = false;
            aids = false;
            startPos[chosen] = cassettes[chosen].transform.localPosition;
        }
    }

}