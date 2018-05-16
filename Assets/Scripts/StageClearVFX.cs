using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum segmentEffects { good, perfect, newSticker };

public class StageClearVFX : MonoBehaviour {

    [SerializeField] private GameObject goodClear;
    [SerializeField] private GameObject perfectClear;
    [SerializeField] private GameObject newSticker;
    [SerializeField] private float timeToShowGood = 1.8f;
    [SerializeField] private float timeToShowPerfect = 2f;
    [SerializeField] private float timeToShowNew = 1.8f;


    /// <summary>
    /// Needs explosions added for even more cool effects. FIX THIS FUCKING SHIT ALREADY YOU IDIOT!
    /// </summary>


    void Update()
    {
        // For testing purposes
        if (Input.GetKeyDown("up"))
            CallFullEffect(perfectClear, timeToShowPerfect);

        // For testing purposes
        if (Input.GetKeyDown("left"))
            CallFullEffect(goodClear, timeToShowGood);

        // For testing purposes
        if (Input.GetKeyDown("right"))
            CallFullEffect(newSticker, timeToShowNew);
    }

    public void CallVFX(segmentEffects typeOfEffect)
    {
        if (typeOfEffect == segmentEffects.good)
            CallFullEffect(goodClear, timeToShowGood);

        if (typeOfEffect == segmentEffects.perfect)
            CallFullEffect(perfectClear, timeToShowPerfect);

        if (typeOfEffect == segmentEffects.newSticker)
            CallFullEffect(newSticker, timeToShowNew);
    }

    private void CallFullEffect(GameObject gameObjectForEffect, float timeToShowEffect)
    {
        GameObject effect = Instantiate(gameObjectForEffect, gameObject.transform);
        Destroy(effect, timeToShowEffect);
    }

}
