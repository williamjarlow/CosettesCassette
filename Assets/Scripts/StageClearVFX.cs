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
    [SerializeField] private Transform positionForParticleEffects;
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
            CallFullEffect(perfectClearAnimation, timeToShowPerfect, perfectParticleEffect);

        // For testing purposes
        if (Input.GetKeyDown("left"))
            CallFullEffect(goodClearAnimation, timeToShowGood, goodParticleEffect);

        // For testing purposes
        if (Input.GetKeyDown("right"))
            CallFullEffect(newStickerAnimation, timeToShowNew, newStickerParticleEffect);
    }

    public void CallVFX(segmentEffects typeOfEffect)
    {
        if (typeOfEffect == segmentEffects.good)
            CallFullEffect(goodClearAnimation, timeToShowGood);

        if (typeOfEffect == segmentEffects.perfect)
            CallFullEffect(perfectClearAnimation, timeToShowPerfect);

        if (typeOfEffect == segmentEffects.newSticker)
            CallFullEffect(newStickerAnimation, timeToShowNew);
    }

    private void CallFullEffect(GameObject gameObjectForEffect, float timeToShowEffect)
    {
        GameObject effect = Instantiate(gameObjectForEffect, gameObject.transform);
        Destroy(effect, timeToShowEffect);
    }

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

}
