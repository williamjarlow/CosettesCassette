using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CassetteAnimation : MonoBehaviour {

    private AudioManager audioManager;
    private AudioTimeline audioTimeline;

    GameManager gameManager;

    public bool cassetteAnimation = true;
    [SerializeField] private SkinnedMeshRenderer dummyRightWheel;
    [SerializeField] private SkinnedMeshRenderer dummyLeftWheel;
    [SerializeField] private SkinnedMeshRenderer animRightWheel;
    [SerializeField] private SkinnedMeshRenderer animLeftWheel;
    [SerializeField] private SkinnedMeshRenderer rightRotator;
    [SerializeField] private SkinnedMeshRenderer leftRotator;
    [SerializeField] private GameObject cassetteToAnimate;
    [SerializeField] private GameObject cassetteToRotate;

    [SerializeField] private GameObject lid;
    [SerializeField] private float cassetteMoveDelay;
    [SerializeField] private int lidClosingDelay;
    [SerializeField] private int startClosingDelay;
    private bool tapeside = false;
    void Start ()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        StartCoroutine(LidClosing());
        audioManager = gameManager.audioManager;
        audioTimeline = gameManager.audioManager.GetComponent<AudioTimeline>();
        
        Debug.Assert(dummyRightWheel != null, "Dummy right wheel is not attached to the script");
        Debug.Assert(dummyLeftWheel != null, "Dummy left wheel is not attached to the script");
        Debug.Assert(animRightWheel != null, "Animation cassette right wheel is not attached to the script");
        Debug.Assert(animLeftWheel != null, "Animation cassette left wheel is not attached to the script");
        Debug.Assert(rightRotator != null, "Right wheel is not attached to the script");
        Debug.Assert(leftRotator != null, "Left wheel is not attached to the script");
        Debug.Assert(cassetteToAnimate != null, "Cassette to animate is not attachd to the script");
        Debug.Assert(cassetteToRotate != null, "Cassette to rotate is not attachd to the script");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (audioManager.startedMusic)
        {
            dummyLeftWheel.SetBlendShapeWeight(0, (audioManager.GetTimeLinePosition() / audioManager.GetTrackLength()) * 100); // * 100 to get the percentages
            dummyRightWheel.SetBlendShapeWeight(0, (audioManager.GetTimeLinePosition() / audioManager.GetTrackLength()) * 100); // * 100 to get the percentages
        }

        if (cassetteAnimation && !audioManager.pausedMusic && audioManager.startedMusic)
            CheckSongSpeed();
    }


    private void CheckSongSpeed()
    {
        // Moving fast forward (Inverted if listening to audiolog)
        if (audioTimeline.movingFast && audioTimeline.movingForward)
        {
            if (audioManager.switchedToAudioLog)
                RotationControl("fastBackwards");
            else
            RotationControl("fastForward");
        }

        // Moving fast backwards (Inverted if listening to audiolog)
        if (audioTimeline.movingFast && !audioTimeline.movingForward)
        {
            if (audioManager.switchedToAudioLog)
                RotationControl("fastForward");
            else
                RotationControl("fastBackwards");
        }

        // Moving slow forward (Inverted if listening to audiolog)
        if (audioTimeline.movingSlow && audioTimeline.movingForward)
        {
            if (audioManager.switchedToAudioLog)
                RotationControl("slowBackwards");
            else
                RotationControl("slowForward");
        }

        // Moving slow backwards (Inverted if listening to audiolog)
        if (audioTimeline.movingSlow && !audioTimeline.movingForward)
        {
            if (audioManager.switchedToAudioLog)
                RotationControl("slowForward");
            else
                RotationControl("slowBackwards");
        }

        // Moving forward normally (Inverted if listening to audiolog)
        if (!audioTimeline.movingSlow && !audioTimeline.movingFast)
        {
            if (audioManager.switchedToAudioLog)
                RotationControl("normalBackwards");
            else
                RotationControl("normal");
        }

    }

    private void RotationControl(string kindOfRotation)
    {
        switch (kindOfRotation)
        {
            case "fastForward":
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                dummyRightWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                dummyLeftWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                break;

            case "fastBackwards":
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                dummyRightWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                dummyLeftWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                break;

            case "slowForward":
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                dummyRightWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                dummyLeftWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                break;

            case "slowBackwards":
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                dummyRightWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                dummyLeftWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                break;

            case "normalBackwards":
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
                dummyRightWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
                dummyLeftWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
                break;

            default:
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
                dummyRightWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
                dummyLeftWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
                break;
        }
    }

    public void CopyBlendShapeValues()
    {
        if (audioManager.switchedToAudioLog)
        {
            animLeftWheel.SetBlendShapeWeight(0, dummyLeftWheel.GetBlendShapeWeight(0));
            animRightWheel.SetBlendShapeWeight(0, dummyRightWheel.GetBlendShapeWeight(0));
            dummyLeftWheel.transform.ResetRotation();
            dummyRightWheel.transform.ResetRotation();
        }

        if (!audioManager.switchedToAudioLog)
        {
            animLeftWheel.SetBlendShapeWeight(0, 100-dummyLeftWheel.GetBlendShapeWeight(0));
            animRightWheel.SetBlendShapeWeight(0, 100-dummyRightWheel.GetBlendShapeWeight(0));
            dummyLeftWheel.transform.ResetRotation();
            dummyRightWheel.transform.ResetRotation();
        }
    }


    // Designer code below
    public void PlayAnimation()
    {
        lid.GetComponent<Animator>().SetBool("Run", false);
        StartCoroutine(LidOpening());
    }
    private IEnumerator LidOpening ()
    {
        yield return new WaitForSeconds(cassetteMoveDelay);
        cassetteToRotate.SetActive(false);
        cassetteToAnimate.SetActive(true);
        cassetteToAnimate.GetComponent<Animator>().SetBool("Intro", true);
        cassetteToAnimate.GetComponent<Animator>().SetBool("Run", true);
        if (tapeside == false)
        {
            tapeside = true; 
        }
        else if (tapeside == true)
        {
            tapeside = false;
            cassetteToAnimate.GetComponent<Animator>().SetBool("isOnA", false);
        }
        yield return new WaitForSeconds(lidClosingDelay);
        lid.GetComponent<Animator>().SetBool("Run", true);
        cassetteToAnimate.SetActive(false);
        cassetteToRotate.SetActive(true);
        cassetteToRotate.transform.Rotate(0,180,0);
        
    }
    private IEnumerator LidClosing()
    {
        yield return new WaitForSeconds(startClosingDelay);
        
        cassetteToAnimate.SetActive(false);
        cassetteToRotate.SetActive(true);
    }
}
    