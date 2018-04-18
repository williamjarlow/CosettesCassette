using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioDistortion : MonoBehaviour
{

    private AudioManager audioManager;
    float currentDistortion;

    void Awake()
    {
        audioManager = GetComponent<AudioManager>();
    }

    private void Update()
    {
    }

    public float GetDistortion()
    {
        float temp;
        temp = currentDistortion;
        return temp;
    }

    public void SetDistortion(float newDistortion)
    {
        currentDistortion = newDistortion;
        audioManager.gameMusicEv.setParameterValue("dist_level", currentDistortion);
    }
}