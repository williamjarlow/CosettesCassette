using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum segmentEffects { good, perfect, newSticker };

public class StageClearVFX : MonoBehaviour {

    [SerializeField] private GameObject goodClearAnimation;
    [SerializeField] private GameObject goodParticleEffect;
    [SerializeField] private GameObject perfectClearAnimation;
    [SerializeField] private GameObject perfectParticleEffect;
    [SerializeField] private GameObject newStickerAnimation;
    [SerializeField] private GameObject newStickerParticleEffect;
    [Header("OBS! This needs the prefab ParticleSpawnPosition to work")]
    [SerializeField] private Transform positionForParticleEffects;
    [SerializeField] private float timeToShowGood = 1.8f;
    [SerializeField] private float timeToShowPerfect = 2f;
    [SerializeField] private float timeToShowNew = 1.8f;
    [SerializeField] private float goodParticleYOffset = 1.6f;
    [SerializeField] private float perfectParticleYOffset = 1.6f;
    [SerializeField] private float newStickerParticleYOffset = 0.1f;


    private void Start()
    {
        Debug.Assert(positionForParticleEffects != null, "StageClearVFX needs the prefab ParticleSpawnPosition. Make sure you read info on prefab!");
    }
    void Update()
    {
        // For testing purposes
        if (Input.GetKeyDown("up"))
            CallFullEffect(perfectClearAnimation, timeToShowPerfect, perfectParticleEffect, perfectParticleYOffset);

        // For testing purposes
        if (Input.GetKeyDown("left"))
            CallFullEffect(goodClearAnimation, timeToShowGood, goodParticleEffect, goodParticleYOffset);

        // For testing purposes
        if (Input.GetKeyDown("right"))
            CallFullEffect(newStickerAnimation, timeToShowNew, newStickerParticleEffect, newStickerParticleYOffset);
    }

    public void CallVFX(segmentEffects typeOfEffect)
    {
        if (typeOfEffect == segmentEffects.good)
            CallFullEffect(goodClearAnimation, timeToShowGood, goodParticleEffect);

        if (typeOfEffect == segmentEffects.perfect)
            CallFullEffect(perfectClearAnimation, timeToShowPerfect, perfectParticleEffect);

        if (typeOfEffect == segmentEffects.newSticker)
            CallFullEffect(newStickerAnimation, timeToShowNew, newStickerParticleEffect);
    }

    ///
    private void CallFullEffect(GameObject gameObjectForEffect, float timeToShowEffect)
    {
        GameObject effect = Instantiate(gameObjectForEffect, gameObject.transform);
        Destroy(effect, timeToShowEffect);
    }
    ///

    private void CallFullEffect(GameObject gameObjectForEffect, float timeToShowEffect, GameObject particleEffect)
    {
        GameObject effect = Instantiate(gameObjectForEffect, gameObject.transform);
        Destroy(effect, timeToShowEffect);
        foreach (Transform particleEffects in particleEffect.transform)
        {
            if (particleEffects.GetComponent<ParticleSystem>() != null)
            {
                GameObject temp = Instantiate(particleEffects.gameObject, positionForParticleEffects);
                temp.GetComponent<ParticleSystem>().Play();
                Destroy(temp, temp.GetComponent<ParticleSystem>().main.duration);
            }
        }
    }


    // WIP!!!!! For offset
    private void CallFullEffect(GameObject gameObjectForEffect, float timeToShowEffect, GameObject particleEffect, float yOffset)
    {
        GameObject effect = Instantiate(gameObjectForEffect, gameObject.transform);
        Destroy(effect, timeToShowEffect);
        foreach (Transform particleEffects in particleEffect.transform)
        {
            if (particleEffects.GetComponent<ParticleSystem>() != null)
            {
                GameObject temp = Instantiate(particleEffects.gameObject, positionForParticleEffects);
                temp.transform.position = new Vector3(temp.transform.position.x, temp.transform.position.y + yOffset, temp.transform.position.z);
                temp.GetComponent<ParticleSystem>().Play();
                Destroy(temp, temp.GetComponent<ParticleSystem>().main.duration);
            }
        }
    }

}
