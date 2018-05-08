﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CassetteAnimation : MonoBehaviour {

    private AudioManager audioManager;
    private AudioTimeline audioTimeline;
    public bool cassetteAnimation = true;
    [SerializeField] private SkinnedMeshRenderer rightWheel;
    [SerializeField] private SkinnedMeshRenderer leftWheel;
    [SerializeField] private SkinnedMeshRenderer rightRotator;
    [SerializeField] private SkinnedMeshRenderer leftRotator;
    [SerializeField] private GameObject cassetteToAnimate;
    [SerializeField] private GameObject cassetteToRotate;
    [SerializeField] private GameObject lid;
    [SerializeField] private float cassetteMoveDelay;
    [SerializeField] private int lidClosingDelay;
    [SerializeField] private int startClosingDelay;

    void Start ()
    {
        StartCoroutine(LidClosing());
        audioManager = GameManager.Instance.audioManager;
        audioTimeline = GameManager.Instance.audioManager.GetComponent<AudioTimeline>();
        
        Debug.Assert(rightWheel != null, "Right wheel is not attached to the script");
        Debug.Assert(leftWheel != null, "Left wheel is not attached to the script");
        Debug.Assert(rightRotator != null, "Right wheel is not attached to the script");
        Debug.Assert(leftRotator != null, "Left wheel is not attached to the script");
        Debug.Assert(cassetteToAnimate != null, "Cassette to animate is not attachd to the script");
        Debug.Assert(cassetteToRotate != null, "Cassette to rotate is not attachd to the script");
    }
	
	// Update is called once per frame
	void Update ()
    {
        leftWheel.SetBlendShapeWeight(0, (audioManager.GetTimeLinePosition() / audioManager.GetTrackLength()) * 100); // * 100 to get the percentages
        rightWheel.SetBlendShapeWeight(0, (audioManager.GetTimeLinePosition() / audioManager.GetTrackLength()) * 100); // * 100 to get the percentages

        if (cassetteAnimation && !audioManager.pausedMusic)
            CheckSongSpeed();
        
    }


    private void CheckSongSpeed()
    {
        // Moving fast forward
        if (audioTimeline.movingFast && audioTimeline.movingForward)
            RotationControl("fastForward");
        // Moving fast backwards
        if (audioTimeline.movingFast && !audioTimeline.movingForward)
            RotationControl("fastBackwards");
        // Moving slow forward
        if (audioTimeline.movingSlow && audioTimeline.movingForward)
            RotationControl("slowForward");
        // Moving slow backwards
        if (audioTimeline.movingSlow && !audioTimeline.movingForward)
            RotationControl("slowBackwards");
        // Moving forward normally
        if (!audioTimeline.movingSlow && !audioTimeline.movingFast)
            RotationControl("moveNormal");
    }

    private void RotationControl(string kindOfRotation)
    {
        switch (kindOfRotation)
        {
            case "fastForward":
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                rightWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                leftWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                break;

            case "fastBackwards":
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                rightWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                leftWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                break;

            case "slowForward":
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                rightWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                leftWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                break;

            case "slowBackwards":
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                rightWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                leftWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                break;

            default:
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
                rightWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
                leftWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
                break;
        }
    }
    public void PlayAnimation()
    {
        lid.GetComponent<Animator>().SetBool("Run", true);
        StartCoroutine(LidOpening());
    }
    private IEnumerator LidOpening ()
    {
        
        yield return new WaitForSeconds(cassetteMoveDelay);
        cassetteToRotate.SetActive(false);
        cassetteToAnimate.SetActive(true);
        cassetteToAnimate.GetComponent<Animator>().SetBool("Intro", true);
        cassetteToAnimate.GetComponent<Animator>().SetBool("Run", true);
        yield return new WaitForSeconds(lidClosingDelay);
        lid.GetComponent<Animator>().SetBool("Run", false);
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
    