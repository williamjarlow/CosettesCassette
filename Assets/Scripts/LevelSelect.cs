using System.Collections;

using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private GameObject buttons;
    [SerializeField] private Button playButton;
    [SerializeField] private Button aButton;
    [SerializeField] private Button bButton;

    const int levelCount = 7;

    private BoxCollider disablestuff;
    private int selectedLevel;

	private AudioManager audioManager;

    private int currentFocus;
    private int cassetteAmount;

    public List<GameObject> cassettes = new List<GameObject>();
    [Tooltip("The load scene list corresponding to the 'Cassette' list. Use the same index as the 'Cassette' list to set the load scene index")]
    [SerializeField]private List<int> loadSceneIndices = new List<int>();

    private Vector3 origPosition = Vector3.zero;
    private Vector3 touchPosition;

    [Tooltip("Minimum pixels required for the touch to move in x coordinates to switch cassette")]
    [SerializeField] private float minimumMovementForChange = 100;
    [SerializeField] private float rotationDuration;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

    [SerializeField] private float cassetteAngle;
    [HideInInspector] public bool pauseScreen = false;

    [Header("For testing purposes, activate before starting")]
    [SerializeField] private bool workWithMouseInput = false;

    private Vector3[] startPos;
    private bool movementLock = false;

    private SaveSystem saveSystemRef;
    private Dictionary<int, bool> unlocks;

    void Start()
    {
        saveSystemRef = SaveSystem.Instance.GetComponent<SaveSystem>();

        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        cassetteAmount = cassettes.Count;
        currentFocus = cassetteAmount - 1;
        startPos = new Vector3[cassettes.Count];
        for(int i = 0; i < cassetteAmount; i++)
        {
            startPos[i] = cassettes[i].transform.localPosition;
        }

        unlocks = saveSystemRef.GetUnlocks();
        for (int i = cassetteAmount - 2; i > 0; i--)
        {
            int lockFocus = cassetteAmount - i - 1;
            if (unlocks[lockFocus + 2] == true)
            {
                cassettes[i].transform.GetChild(7).gameObject.SetActive(false);
            }
        }

        cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = true;
    }
		
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.U))
        {
            for (int i = 2; i < 9; i++)
                saveSystemRef.UnlockLevel(i);

            unlocks = saveSystemRef.GetUnlocks();
        }

        if (!pauseScreen)
        TouchControls();

        if (workWithMouseInput && !pauseScreen)
        MouseControls();
    }

    private void MouseControls()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.LeftArrow)) // forward
        {
            if (currentFocus >= -1 && currentFocus < cassetteAmount - 1 && movementLock == false && !buttons.activeInHierarchy)
            {
                movementLock = true;
                //Quaternion flatRotation = Quaternion.Euler(270, 0, 0);
                //Quaternion standingRotation = Quaternion.Euler(-cassetteAngle, 0, 0);
                //Quaternion reversedStandingRotation = Quaternion.Euler(180+cassetteAngle, 0, 0);
                StartCoroutine(MoveFromTo(new Vector3(startPos[currentFocus].x, startPos[currentFocus].y + yOffset, startPos[currentFocus].z + xOffset), startPos[currentFocus], rotationDuration, currentFocus));

                for(int i = cassetteAmount - 1; i >= 0; i--)
                {
                    if(i < currentFocus)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y + yOffset, startPos[i].z + xOffset), startPos[i], rotationDuration, i));
                    }
                    else if(i > currentFocus + 1)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y - yOffset, startPos[i].z + xOffset), startPos[i], rotationDuration, i));
                    }
                    else if (i == currentFocus + 1)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y - yOffset, startPos[i].z + xOffset), startPos[i], rotationDuration, i));
                    }
                }

                // Set the previous cassette to not focused
                cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = false;
                currentFocus++;
                // Set the currently focused cassette to focused
                cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = true;

				audioManager.PlayLevelSelectScroll ();
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.RightArrow) && !buttons.activeInHierarchy) // backwards
        {
            if (currentFocus > 0 && currentFocus <= cassetteAmount && movementLock == false)
            {
                movementLock = true;
                //Quaternion flatRotation = Quaternion.Euler(270, 0, 0);
                //Quaternion standingRotation = Quaternion.Euler(-cassetteAngle, 0, 0);
                //Quaternion reversedStandingRotation = Quaternion.Euler(180+cassetteAngle, 0, 0);
                StartCoroutine(MoveFromTo(new Vector3(startPos[currentFocus].x, startPos[currentFocus].y + yOffset, startPos[currentFocus].z - xOffset), startPos[currentFocus], rotationDuration, currentFocus));

                for (int i = cassetteAmount - 1; i >= 0; i--)
                {
                    if (i < currentFocus - 1)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y - yOffset, startPos[i].z - xOffset), startPos[i], rotationDuration, i));
                    }
                    else if (i > currentFocus)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y + yOffset, startPos[i].z - xOffset), startPos[i], rotationDuration, i));
                    }
                    else if(i == currentFocus - 1)
                    {
                        StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y - yOffset, startPos[i].z - xOffset), startPos[i], rotationDuration, i));
                    }
                }

                // Set the previous cassette to not focused
                cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = false;
                currentFocus--;
                // Set the currently focused cassette to focused
                cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = true;
                
				audioManager.PlayLevelSelectScroll();
            }
        }

        // SceneLoader for mouse input
        if(Input.GetMouseButtonUp(0) && buttons.activeInHierarchy == false)
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
						audioManager.PlayLevelSelectSelect ();
                        selectedLevel = hit.transform.GetComponent<LevelSelectLoadScene>().LoadSceneIndex;
                        disablestuff = hit.transform.GetComponent<BoxCollider>();
                        disablestuff.enabled = false;
                        buttons.SetActive(true);
                        EvaluateAndSetButtonStates(selectedLevel);
                        if (buttons.activeInHierarchy == true)
                        {
                            movementLock = true;
                        }
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
                    if (currentFocus >= -1 && currentFocus < cassetteAmount - 1 && movementLock == false && !buttons.activeInHierarchy)
                    {
                        movementLock = true;
                        StartCoroutine(MoveFromTo(new Vector3(startPos[currentFocus].x, startPos[currentFocus].y + yOffset, startPos[currentFocus].z + xOffset), startPos[currentFocus], rotationDuration, currentFocus));

                        for (int i = cassetteAmount - 1; i >= 0; i--)
                        {
                            if (i < currentFocus)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y + yOffset, startPos[i].z + xOffset), startPos[i], rotationDuration, i));
                            }
                            else if (i > currentFocus + 1)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y - yOffset, startPos[i].z + xOffset), startPos[i], rotationDuration, i));
                            }
                            else if (i == currentFocus + 1)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y - yOffset, startPos[i].z + xOffset), startPos[i], rotationDuration, i));
                            }
                        }

                        // Set the previous cassette to not focused
                        cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = false;
                        currentFocus++;
                        // Set the currently focused cassette to focused
                        cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = true;

                        audioManager.PlayLevelSelectScroll();
                    }
                }

                // If you scrolled to the right
                if ((touchPosition.x - origPosition.x) < -minimumMovementForChange)
                {
                    if (currentFocus > 0 && currentFocus <= cassetteAmount && movementLock == false && !buttons.activeInHierarchy)
                    {
                        movementLock = true;
                        StartCoroutine(MoveFromTo(new Vector3(startPos[currentFocus].x, startPos[currentFocus].y + yOffset, startPos[currentFocus].z - xOffset), startPos[currentFocus], rotationDuration, currentFocus));

                        for (int i = cassetteAmount - 1; i >= 0; i--)
                        {
                            if (i < currentFocus - 1)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y - yOffset, startPos[i].z - xOffset), startPos[i], rotationDuration, i));
                            }
                            else if (i > currentFocus)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y + yOffset, startPos[i].z - xOffset), startPos[i], rotationDuration, i));
                            }
                            else if (i == currentFocus - 1)
                            {
                                StartCoroutine(MoveFromTo(new Vector3(startPos[i].x, startPos[i].y - yOffset, startPos[i].z - xOffset), startPos[i], rotationDuration, i));
                            }
                        }

                        // Set the previous cassette to not focused
                        cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = false;
                        currentFocus--;
                        // Set the currently focused cassette to focused
                        cassettes[currentFocus].GetComponent<LevelSelectLoadScene>().isFocused = true;

                        audioManager.PlayLevelSelectScroll();
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
                                audioManager.PlayLevelSelectSelect ();
								selectedLevel = hit.transform.GetComponent<LevelSelectLoadScene>().LoadSceneIndex;
								disablestuff = hit.transform.GetComponent<BoxCollider>();
								disablestuff.enabled = false;
								buttons.SetActive(true);
                                EvaluateAndSetButtonStates(selectedLevel);
                                if (buttons.activeInHierarchy == true)
								{
									movementLock = true;
								}
                            }
                        }
                    }
                }

                touchPosition = new Vector3(Vector3.zero.x, touchPosition.y, Vector3.zero.z);
                origPosition = -Vector3.one;
            }
        }
    }
    IEnumerator MoveFromTo(Vector3 pointA, Vector3 pointB, float time, int chosen)
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
                yield return new WaitForEndOfFrame();
            }
            moving = false;
            movementLock = false;
            startPos[chosen] = cassettes[chosen].transform.localPosition;
        }
    }

    public void BacktolvlSelect()
    {
        if (buttons.activeInHierarchy == false)
        {
            movementLock = false;
            disablestuff.enabled = true;
			audioManager.PlayLevelSelectBack ();
        }
    }

    public void PlayLevel()
    {
        unlocks = saveSystemRef.GetUnlocks();
        if (unlocks.ContainsKey(selectedLevel))
        {
            if (unlocks[selectedLevel] == true)
            {
                SceneManager.LoadScene(selectedLevel);
				audioManager.PlayLevelSelectPlay();
                audioManager.AudioStopMusic();
				BankManager.Instance.SceneChanged (SceneManager.GetSceneByBuildIndex (selectedLevel).name);
            }
        }
    }

    public void PlayASide()
    {
        unlocks = saveSystemRef.GetUnlocks();
        if (unlocks.ContainsKey(selectedLevel + levelCount))
        {
            if (unlocks[selectedLevel + levelCount] == true)
            {
                selectedLevel = selectedLevel + levelCount;
                SceneManager.LoadScene(selectedLevel);
				audioManager.PlayLevelSelectPlay();
                audioManager.AudioStopMusic();
				BankManager.Instance.SceneChanged (SceneManager.GetSceneByBuildIndex (selectedLevel).name);
            }
        }
    }

    public void PlayBSide()
    {
        unlocks = saveSystemRef.GetUnlocks();
        if (unlocks.ContainsKey(selectedLevel + levelCount * 2))
        {
            if (unlocks[selectedLevel + levelCount * 2] == true)
            {
                selectedLevel = selectedLevel + levelCount * 2;
                SceneManager.LoadScene(selectedLevel);
				audioManager.PlayLevelSelectPlay();
                audioManager.AudioStopMusic();
				BankManager.Instance.SceneChanged (SceneManager.GetSceneByBuildIndex (selectedLevel).name);
            }
        }
    }

    private void EvaluateAndSetButtonStates(int index)
    {
        unlocks = saveSystemRef.GetUnlocks();
        if (unlocks.ContainsKey(index))
        {
            if (unlocks[selectedLevel] == true)
                playButton.interactable = true;
            else
                playButton.interactable = false;
        }
        if (unlocks.ContainsKey(index + levelCount))
        {
            if (unlocks[selectedLevel + levelCount] == true)
                aButton.interactable = true;
            else
                aButton.interactable = false;
        }
        if (unlocks.ContainsKey(index + levelCount * 2))
        {
            if (unlocks[selectedLevel + levelCount * 2] == true)
                bButton.interactable = true;
            else
                bButton.interactable = false;
        }
        if (index >= 23) //If at credits scene
        {
            aButton.interactable = false;
            bButton.interactable = false;
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