using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLatencyTest : MonoBehaviour {

    private bool isPlaying = false;
    [SerializeField] private string bassDrumPath;
    
    public FMOD.Studio.EventInstance eventInstance;

	// Use this for initialization
	void Start () {

        AudioSettings.SetDSPBufferSize(256, 2);
        //eventInstance = FMODUnity.RuntimeManager.CreateInstance(bassDrumPath);
        //var config = AudioSettings.GetConfiguration();
        //config.dspBufferSize = 18;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.LogError("beat: " + Time.time.ToString());

            //AudioSource audio = GetComponent<AudioSource>();
            //audio.Play();
        }

	}

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isPlaying)
            {
                isPlaying = true;
                Debug.LogError("beat fixed: " + Time.time.ToString());

                AudioSource audio = GetComponent<AudioSource>();
                audio.Play();
                //eventInstance.start();
                //FMODUnity.RuntimeManager.PlayOneShot(bassDrumPath);


                StartCoroutine(ResetPlayed());
            }
        }
    }

    private IEnumerator ResetPlayed()
    {
        yield return new WaitForSeconds(0.05f);
        isPlaying = false;
    }
} 
