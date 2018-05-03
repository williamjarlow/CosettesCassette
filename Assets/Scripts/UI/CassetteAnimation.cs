using System.Collections;
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
    [SerializeField] private GameObject Cassette;
    [SerializeField] private GameObject Lid;
    [SerializeField] private float CassetteMoveDelay;
    [SerializeField] private int lidClosingDelay;
    void Start ()
    {
        audioManager = GameManager.Instance.audioManager;
        audioTimeline = GameManager.Instance.audioManager.GetComponent<AudioTimeline>();

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

        if(cassetteAnimation && !audioManager.pausedMusic)           // FIXA SWITCH ISTÄLLET, SATANS JÄVLA IF. Kanske fixa en listener istället. Mycket konstant lyssnande på ändringar.
        {
            // Moving fast forward
            if (audioTimeline.movingFast && audioTimeline.movingForward)
            {
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
            }
            // Moving fast backwards
            if (audioTimeline.movingFast && !audioTimeline.movingForward)
            {
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteFastSpeed * Time.deltaTime)));
            }
            // Moving slow forward
            if (audioTimeline.movingSlow && audioTimeline.movingForward)
            {
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
            }
            // Moving slow backwards
            if (audioTimeline.movingSlow && !audioTimeline.movingForward)
            {
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z - (audioTimeline.cassetteSlowSpeed * Time.deltaTime)));
            }
            // Moving forward normally
            if (!audioTimeline.movingSlow && !audioTimeline.movingFast)
            {
                rightRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
                leftRotator.transform.Rotate(new Vector3(0, 0, transform.rotation.z + (audioTimeline.cassetteNormalSpeed * Time.deltaTime)));
            }



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
    