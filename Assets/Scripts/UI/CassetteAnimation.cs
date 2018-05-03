﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CassetteAnimation : MonoBehaviour {

    private AudioManager audioManager;
    public bool cassetteAnimation = true;
    [SerializeField] private SkinnedMeshRenderer rightWheel;
    [SerializeField] private SkinnedMeshRenderer leftWheel;
    [SerializeField] private SkinnedMeshRenderer rightRotator;
    [SerializeField] private SkinnedMeshRenderer leftRotator;
    [SerializeField] private GameObject Cassette;
    [SerializeField] private GameObject Lid;
    [SerializeField] private float CassetteMoveDelay;
    [SerializeField] private int lidClosingDelay;
    void Start ()
    {
        audioManager = GameManager.Instance.audioManager;

        Debug.Assert(rightWheel != null, "Right wheel is not attached to the script");
        Debug.Assert(leftWheel != null, "Left wheel is not attached to the script");
        Debug.Assert(rightRotator != null, "Right wheel is not attached to the script");
        Debug.Assert(leftRotator != null, "Left wheel is not attached to the script");
    }
	
	// Update is called once per frame
	void Update ()
    {
        leftWheel.SetBlendShapeWeight(0, (audioManager.GetTimeLinePosition() / audioManager.GetTrackLength()) * 100); // * 100 to get the percentages
        rightWheel.SetBlendShapeWeight(0, (audioManager.GetTimeLinePosition() / audioManager.GetTrackLength()) * 100); // * 100 to get the percentages

        if(cassetteAnimation)
        {
            //rightWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z - 1));
            //leftWheel.transform.Rotate(new Vector3(0, 0, transform.rotation.z - 1));
            rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + 1));
            leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + 1));
        }
    }
    public void PlayAnimation()
    {
        
        Lid.GetComponent<Animator>().SetBool("Run", true);
        StartCoroutine(LidOpening());
    }
    private IEnumerator LidOpening ()
    {

        yield return new WaitForSeconds(CassetteMoveDelay);
        Cassette.GetComponent<Animator>().SetBool("Run", true);
        yield return new WaitForSeconds(lidClosingDelay);
        Lid.GetComponent<Animator>().SetBool("Run", false);
        
    }
    
}
    