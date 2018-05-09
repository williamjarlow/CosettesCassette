using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum segmentEffects { good, perfect, newRecord };

public class SegmentClearVFX : MonoBehaviour {

    [SerializeField] private GameObject goodClear;
    [SerializeField] private GameObject perfectClear;
    [SerializeField] private GameObject newRecord;
    [SerializeField] private float timeToShowEffect = 2f;


    void Update()
    {
        // For testing purposes
        if (Input.GetKeyDown("space"))
            CallFullEffect(perfectClear);
    }

    public void CallSegmentVFX(segmentEffects typeOfEffect)
    {
        if (typeOfEffect == segmentEffects.good)
            CallFullEffect(goodClear);

        if (typeOfEffect == segmentEffects.perfect)
            CallFullEffect(perfectClear);

        if (typeOfEffect == segmentEffects.newRecord)
            CallFullEffect(newRecord);
    }

    private void CallFullEffect(GameObject gameObjectForEffect)
    {
        GameObject effect = Instantiate(gameObjectForEffect, gameObject.transform);
        Destroy(effect, timeToShowEffect);
    }


}
