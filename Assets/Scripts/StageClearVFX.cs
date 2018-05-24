using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum segmentEffects { good, perfect, newSticker };

public class StageClearVFX : MonoBehaviour
{
    [SerializeField] private GameObject goodClearAnimation;
    [SerializeField] private GameObject goodParticleEffect;
    [SerializeField] private GameObject perfectClearAnimation;
    [SerializeField] private GameObject perfectParticleEffect;
    [SerializeField] private GameObject newStickerAnimation;
    [SerializeField] private GameObject newStickerParticleEffect;
    [Header("OBS! This needs the prefab ParticleSpawnPosition in the scene to work")]
    [SerializeField] private Transform positionForParticleEffects;
    public float timeToShowGood = 3;
    public float timeToShowPerfect = 4.5f;
    public float timeToShowNew = 3;
    [SerializeField] private float goodParticleYOffset = 1.6f;
    [SerializeField] private float perfectParticleYOffset = 1.6f;
    [SerializeField] private float newStickerParticleYOffset = 0.1f;
    [SerializeField] private GameObject stickerSpritePosition;
    private SpriteRenderer stickerSprite;
	private AudioManager audioManager;

    private void Start()
    {
        Debug.Assert(positionForParticleEffects != null, "StageClearVFX needs the prefab ParticleSpawnPosition. Make sure you read info on prefab!");
        stickerSprite = stickerSpritePosition.GetComponent<SpriteRenderer>();
		audioManager = FindObjectOfType<AudioManager>();
    }

    public void CallVFX(segmentEffects typeOfEffect)
    {
        if (typeOfEffect == segmentEffects.good)
            CallFullEffect(goodClearAnimation, timeToShowGood, goodParticleEffect, goodParticleYOffset);

        if (typeOfEffect == segmentEffects.perfect)
            CallFullEffect(perfectClearAnimation, timeToShowPerfect, perfectParticleEffect, perfectParticleYOffset);

        if (typeOfEffect == segmentEffects.newSticker)
            CallFullEffect(newStickerAnimation, timeToShowNew, newStickerParticleEffect, newStickerParticleYOffset);

        else return;
    }

    public void CallVFXWithStickerEarned(segmentEffects typeOfEffect, Sprite sticker)
    {
        stickerSprite.sprite = sticker;

        if (typeOfEffect == segmentEffects.good)
        {
            CallFullEffect(goodClearAnimation, timeToShowGood, goodParticleEffect, goodParticleYOffset);
            StartCoroutine(StickerDelay(timeToShowGood));
        }

        if (typeOfEffect == segmentEffects.perfect)
        {
            CallFullEffect(perfectClearAnimation, timeToShowPerfect, perfectParticleEffect, perfectParticleYOffset);
            StartCoroutine(StickerDelay(timeToShowPerfect));
        }

        else return;
    }

    public void CallVFXWith2StickersEarned(Sprite sticker, Sprite secondSticker)
    {
        stickerSprite.sprite = sticker;
        CallFullEffect(perfectClearAnimation, timeToShowPerfect, perfectParticleEffect, perfectParticleYOffset);
        StartCoroutine(StickerDelay(timeToShowPerfect));
        StartCoroutine(StickerDelay(timeToShowPerfect + timeToShowNew + 0.1f, secondSticker));
    }

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

    IEnumerator StickerDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        stickerSpritePosition.SetActive(true);
        CallFullEffect(newStickerAnimation, timeToShowNew, newStickerParticleEffect, newStickerParticleYOffset);
		audioManager.PlayStickerGet ();
        StartCoroutine(HideStickerObject(timeToShowNew));
    }

    IEnumerator StickerDelay(float delay, Sprite secondSticker)
    {
        yield return new WaitForSeconds(delay);
        stickerSprite.sprite = secondSticker;
        stickerSpritePosition.SetActive(true);
        CallFullEffect(newStickerAnimation, timeToShowNew, newStickerParticleEffect, newStickerParticleYOffset);
		audioManager.PlayStickerGet ();
        StartCoroutine(HideStickerObject(timeToShowNew));
    }


    // God, so many bad solutions when crunch....
    IEnumerator HideStickerObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        stickerSpritePosition.SetActive(false);
    }

}
