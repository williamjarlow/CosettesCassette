using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CassetteAnimation : MonoBehaviour {

    private AudioManager audioManager;
    public bool cassetteAnimation = true;
    [SerializeField] private SkinnedMeshRenderer rightWheel;
    [SerializeField] private SkinnedMeshRenderer leftWheel;
    void Start ()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        Debug.Assert(rightWheel != null, "Right wheel is not attached to the script");
        Debug.Assert(leftWheel != null, "Left wheel is not attached to the script");
    }
	
	// Update is called once per frame
	void Update ()
    {
        leftWheel.SetBlendShapeWeight(0, (audioManager.GetTimeLinePosition() / audioManager.GetTrackLength()) * 100); // * 100 to get the percentages
        rightWheel.SetBlendShapeWeight(0, (audioManager.GetTimeLinePosition() / audioManager.GetTrackLength()) * 100); // * 100 to get the percentages

        if(cassetteAnimation)
        {
            rightWheel.transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z - 1));
            leftWheel.transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z - 1));
        }
    }
}
