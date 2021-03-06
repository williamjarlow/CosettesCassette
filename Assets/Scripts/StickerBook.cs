using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerBook : MonoBehaviour {

    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backwardsButton;
    [SerializeField] private Image[] images;
    [SerializeField] private Text[] titles;
    [SerializeField] private Text[] descriptions;
    private StickerManager stickerManRef;
	private AudioManager audioManager;
    private int stickerAmount = 6;
    private int stickerSet = 0;
    
    void Awake()
    {
        stickerManRef = SaveSystem.Instance.transform.GetChild(0).GetComponent<StickerManager>();
		audioManager = GameObject.FindGameObjectWithTag ("AudioManager").GetComponent<AudioManager> ();
    }

    public void InitializeStickerPage()
    {
        forwardButton.SetActive(true);
        for (int i = 0; i < stickerAmount; i++)
        {
            images[i].sprite = stickerManRef.stickers[i].Sprite;
            titles[i].text = stickerManRef.stickers[i].Name;
            descriptions[i].text = stickerManRef.stickers[i].Description;
            if(stickerManRef.stickers[i].Unlocked == false)
            {
                images[i].color = Color.black;
                descriptions[i - stickerSet * stickerAmount].text = "";
            }
            else
            {
                images[i].color = Color.white;
            }
        }
    }

    public void FlipPagesForward()
    {
        if (stickerSet * stickerAmount < stickerManRef.stickers.Length - stickerAmount)
        {
            backwardsButton.SetActive(true);
			audioManager.PlayScriptFlip ();
            stickerSet += 1;
            for (int i = stickerSet * stickerAmount; i < stickerAmount * (stickerSet + 1); i++)
            {
                if(i < stickerManRef.stickers.Length)
                {
                    images[i - stickerSet * stickerAmount].sprite = stickerManRef.stickers[i].Sprite;
                    titles[i - stickerSet * stickerAmount].text = stickerManRef.stickers[i].Name;
                    descriptions[i - stickerSet * stickerAmount].text = stickerManRef.stickers[i].Description;

                    if (stickerManRef.stickers[i].Unlocked == false)
                    {
                        images[i - stickerSet * stickerAmount].color = Color.black;
                        descriptions[i - stickerSet * stickerAmount].text = "";
                    }
                    else
                    {
                        images[i - stickerSet * stickerAmount].color = Color.white;
                    }
                }
                else
                {
                    images[i - stickerSet * stickerAmount].sprite = null;
                    images[i - stickerSet * stickerAmount].color = new Vector4(images[i - stickerSet * stickerAmount].color.r, images[i - stickerSet * stickerAmount].color.g, images[i - stickerSet * stickerAmount].color.b, 0);
                    titles[i - stickerSet * stickerAmount].text = "";
                    descriptions[i - stickerSet * stickerAmount].text = "";
                }
            }
        }
        if (stickerSet * stickerAmount >= stickerManRef.stickers.Length - stickerAmount)
            forwardButton.SetActive(false);
    }

    public void FlipPagesBackward()
    {
        if (stickerSet * stickerAmount > 0)
        {
            forwardButton.SetActive(true);
			audioManager.PlayScriptFlip ();
            stickerSet -= 1;
            for (int i = stickerSet * stickerAmount; i < stickerAmount * (stickerSet + 1); i++)
            {
                if (i < stickerManRef.stickers.Length)
                {
                    images[i - stickerSet * stickerAmount].color = new Vector4(images[i - stickerSet * stickerAmount].color.r, images[i - stickerSet * stickerAmount].color.g, images[i - stickerSet * stickerAmount].color.b, 1);
                    images[i - stickerSet * stickerAmount].sprite = stickerManRef.stickers[i].Sprite;
                    titles[i - stickerSet * stickerAmount].text = stickerManRef.stickers[i].Name;
                    descriptions[i - stickerSet * stickerAmount].text = stickerManRef.stickers[i].Description;

                    if (stickerManRef.stickers[i].Unlocked == false)
                    {
                        images[i - stickerSet * stickerAmount].color = Color.black;
                        descriptions[i - stickerSet * stickerAmount].text = "";
                    }
                    else
                    {
                        images[i - stickerSet * stickerAmount].color = Color.white;
                    }
                }
                else
                {
                    images[i - stickerSet * stickerAmount].sprite = null;
                    images[i - stickerSet * stickerAmount].color = new Vector4(images[i - stickerSet * stickerAmount].color.r, images[i - stickerSet * stickerAmount].color.g, images[i - stickerSet * stickerAmount].color.b, 0);
                    titles[i - stickerSet * stickerAmount].text = "";
                    descriptions[i - stickerSet * stickerAmount].text = "";
                }
            }
        }
        if (stickerSet < 1)
            backwardsButton.SetActive(false);
    }

}
