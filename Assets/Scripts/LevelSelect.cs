using System.Collections;

using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;



public class LevelSelect : MonoBehaviour
{

    private int currentFocus;
    private int cassetteAmount;
    [SerializeField] private int rotationAmount = 50;

    public List<GameObject> cassettes = new List<GameObject>();
    [Tooltip("The load scene list corresponding to the 'Cassette' list. Use the same index as the 'Cassette' list to set the load scene index")]
    [SerializeField]private List<int> loadSceneIndices = new List<int>();

    private Vector3 origPosition = Vector3.zero;
    private Vector3 touchPosition;

    [Tooltip("Minimum pixels required for the touch to move in x coordinates to switch cassette")]
    [SerializeField] private float minimumMovementForChange = 100;
    [SerializeField] private float rotationDuration;
    [SerializeField] private float middleOffset;
    [SerializeField] private float cassetteOffset;
    [SerializeField] private float cassetteAngle;

    // Temporary
    ////
    [Header("For testing purposes, activate before starting")]
    [SerializeField] private bool workWithMouseInput = false;
    ////

    public bool pauseScreen = false;

    private Vector3[] startPos;
    private bool movementLock = false;


                                     // ** TODO ** // 
        // 
        // 1. Make it so you can only call the function LoadScene on the object that is focused

    void Start()
    {
        cassetteAmount = cassettes.Count;
        currentFocus = cassetteAmount;
        currentFocus = 0;
        startPos = new Vector3[cassettes.Count];
        for(int i = 0; i < cassetteAmount; i++)
        {
            startPos[i] = cassettes[i].transform.localPosition;
        }

    }


    void Update()
    {
        if (!pauseScreen)
        TouchControls();


        // Temporary
        ////
        if (workWithMouseInput && !pauseScreen)
        MouseControls();
        ////

            
    }

    private void MouseControls()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            if (currentFocus >= -1 && currentFocus < cassetteAmount - 1 && movementLock == false)
            {

                // Set the previous cassette to not focused
                cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = false;
                currentFocus++;
                // Set the currently focused cassette to focused
                cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = true;

                movementLock = true;
                Quaternion flatRotation = Quaternion.Euler(270, 0, 0);
                Quaternion standingRotation = Quaternion.Euler(-cassetteAngle, 0, 0);
                Quaternion reversedStandingRotation = Quaternion.Euler(180+cassetteAngle, 0, 0);
                StartCoroutine(MoveFromTo(new Vector3(startPos[currentFocus].x, startPos[currentFocus].y, startPos[currentFocus].z + middleOffset), startPos[currentFocus], rotationDuration, currentFocus, flatRotation, reversedStandingRotation));

                for(int i = cassetteAmount - 1; i > 0; i--)
                {
                    if(i < currentFocus - 1)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z + cassetteOffset), startPos[i], rotationDuration, i, standingRotation, standingRotation));
                    }
                    else if(i > currentFocus)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z + cassetteOffset), startPos[i], rotationDuration, i, reversedStandingRotation, reversedStandingRotation));
                    }
                    else if (i == currentFocus - 1)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z + middleOffset), startPos[i], rotationDuration, i, standingRotation, flatRotation));
                    }
                }
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            if (currentFocus > 0 && currentFocus <= cassetteAmount && movementLock == false)
            {

                // Set the previous cassette to not focused
                cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = false;
                currentFocus++;
                // Set the currently focused cassette to focused
                cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = true;

                movementLock = true;
                Quaternion flatRotation = Quaternion.Euler(270, 0, 0);
                Quaternion standingRotation = Quaternion.Euler(-cassetteAngle, 0, 0);
                Quaternion reversedStandingRotation = Quaternion.Euler(180+cassetteAngle, 0, 0);
                StartCoroutine(MoveFromTo(new Vector3(startPos[currentFocus].x, startPos[currentFocus].y, startPos[currentFocus].z - middleOffset), startPos[currentFocus], rotationDuration, currentFocus, flatRotation, standingRotation));

                for (int i = cassetteAmount - 1; i > 0; i--)
                {
                    if (i < currentFocus)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z - cassetteOffset), startPos[i], rotationDuration, i, standingRotation, standingRotation));
                    }
                    else if (i > currentFocus+1)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z - cassetteOffset), startPos[i], rotationDuration, i, reversedStandingRotation, reversedStandingRotation));
                    }
                    else if(i == currentFocus+1)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z - middleOffset), startPos[i], rotationDuration, i, reversedStandingRotation, flatRotation));
                    }
                }
            }
        }

        // SceneLoader for mouse input
        if(Input.GetMouseButtonUp(0))
        {
            // Raycast the ended touch position
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // If an object was hit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // If the hit object actually has LevelSelectLoadScene, i.e is a cassette
                if(hit.transform.GetComponent<LevelSelectLoadScene>() != null)
                {   
                    // If the hit cassette is focused
                    if (hit.transform.GetComponent<LevelSelectLoadScene>().isFocused == true)
                    {
                        SceneManager.LoadScene(hit.transform.GetComponent<LevelSelectLoadScene>().LoadSceneIndex);
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

            touchPosition = myTouch.position;

            if (myTouch.phase == TouchPhase.Began)
            {
                origPosition = touchPosition;
            }
            if (myTouch.phase == TouchPhase.Ended)
            {
                touchPosition = myTouch.position;

                // If you scrolled to the left
                if ((touchPosition.x - origPosition.x) >= minimumMovementForChange)
                {
                    if (currentFocus >= -1 && currentFocus <= cassetteAmount - 1 && movementLock == false)
                    {
                        
                        // Set the previous cassette to not focused
                        cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = false;
                        currentFocus++;
                        // Set the currently focused cassette to focused
                        cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = true;

                        movementLock = true;
                        Quaternion flatRotation = Quaternion.Euler(270, 0, 0);
                        Quaternion standingRotation = Quaternion.Euler(-cassetteAngle, 0, 0);
                        Quaternion reversedStandingRotation = Quaternion.Euler(180 + cassetteAngle, 0, 0);
                        StartCoroutine(MoveFromTo(new Vector3(startPos[currentFocus].x, startPos[currentFocus].y, startPos[currentFocus].z + middleOffset), startPos[currentFocus], rotationDuration, currentFocus, flatRotation, reversedStandingRotation));

                        for (int i = cassetteAmount - 1; i > 0; i--)
                        {
                            if (i < currentFocus - 1)
                            
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z + cassetteOffset), startPos[i], rotationDuration, i, standingRotation, standingRotation));
                            }
                            else if (i > currentFocus)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z + cassetteOffset), startPos[i], rotationDuration, i, reversedStandingRotation, reversedStandingRotation));
                            }
                            else if (i == currentFocus - 1)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z + middleOffset), startPos[i], rotationDuration, i, standingRotation, flatRotation));
                            }
                        }
                    }
                }

                // If you scrolled to the right
                if ((touchPosition.x - origPosition.x) < -minimumMovementForChange)
                {
                    if (currentFocus > 0 && currentFocus <= cassetteAmount && movementLock == false)
                    {

                        // Set the previous cassette to not focused
                        cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = false;
                        currentFocus++;
                        // Set the currently focused cassette to focused
                        cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = true;

                        movementLock = true;
                        Quaternion flatRotation = Quaternion.Euler(270, 0, 0);
                        Quaternion standingRotation = Quaternion.Euler(-cassetteAngle, 0, 0);
                        Quaternion reversedStandingRotation = Quaternion.Euler(180 + cassetteAngle, 0, 0);
                        StartCoroutine(MoveFromTo(new Vector3(startPos[currentFocus].x, startPos[currentFocus].y, startPos[currentFocus].z - middleOffset), startPos[currentFocus], rotationDuration, currentFocus, flatRotation, standingRotation));

                        for (int i = cassetteAmount - 1; i > 0; i--)
                        {
                            if (i < currentFocus)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z - cassetteOffset), startPos[i], rotationDuration, i, standingRotation, standingRotation));
                            }
                            else if (i > currentFocus + 1)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z - cassetteOffset), startPos[i], rotationDuration, i, reversedStandingRotation, reversedStandingRotation));
                            }
                            else if (i == currentFocus + 1)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y, startPos[i].z - middleOffset), startPos[i], rotationDuration, i, reversedStandingRotation, flatRotation));
                            }
                        }
                    }
                }

                // If we didnt move the touch far enough, i.e clicked
                if(Mathf.Abs(touchPosition.x - origPosition.x) < minimumMovementForChange)
                {
                    // Raycast the ended touch position
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(touchPosition);

                    // If an object was hit
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        // If the hit object actually has LevelSelectLoadScene, i.e is a cassette
                        if (hit.transform.GetComponent<LevelSelectLoadScene>() != null)
                        {
                            // If the hit cassette is focused
                            if (hit.transform.GetComponent<LevelSelectLoadScene>().isFocused == true)
                            {
                                SceneManager.LoadScene(hit.transform.GetComponent<LevelSelectLoadScene>().LoadSceneIndex);
                            }
                        }

                    }
                }

                touchPosition = new Vector3(Vector3.zero.x, touchPosition.y, Vector3.zero.z);
                origPosition = -Vector3.one;
            }
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
            movementLock = false;
            startPos[chosen] = cassettes[chosen].transform.localPosition;
        }
    }


    public void OpenPause()
    {
        pauseScreen = true;
    }

    public void ClosePause()
    {
        pauseScreen = false;
    }

}