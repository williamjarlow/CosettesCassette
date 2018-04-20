﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerManager : MonoBehaviour {

    //[SerializeField] private GameObject stickerPrefab;
    //[SerializeField] private GameObject generalCategory;

    private StickerButton activeButton;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject generalBtn;
    [SerializeField] private GameObject stickerMenu;
    [SerializeField] private GameObject visualSticker;
    [SerializeField] private GameObject earningSticker;
    [SerializeField] private GameObject particleSticker;

    [SerializeField] private GameObject stickerPrefab;

    public Dictionary<string, Sticker> stickers = new Dictionary<string, Sticker>();

    // Use this for initialization
    void Start ()
    {

        activeButton = generalBtn.GetComponent<StickerButton>();
        activeButton.OnClick();

        stickerMenu.SetActive(false);
        
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.S))
        {
            stickerMenu.SetActive(!stickerMenu.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            EarnSticker("PressQ");
        }
    }

    public void EarnSticker(string title)
    {
        if(stickers[title].EarnSticker())
        {
            GameObject sticker = particleSticker;
            sticker.SetActive(true);
            SetVisualSticker(sticker, title);
            StartCoroutine(HideSticker(sticker));
        }
    }

    public IEnumerator HideSticker(GameObject sticker)
    {
        yield return new WaitForSeconds(3);
        sticker.SetActive(false);
    }

    public void SetVisualSticker(GameObject sticker, string title)
    {
        //sticker.transform.SetParent(sticker.transform);
        Vector3 temp = sticker.transform.GetChild(4).GetComponent<Image>().rectTransform.localPosition;
        temp.x = 0;
        temp.y = 20;
        //temp.z = 0;       //Ballar ur i min scne, kan behövas i ett bättre kanvas
        sticker.transform.GetChild(4).GetComponent<Image>().rectTransform.localPosition = temp;
        //sticker.transform.GetChild(0).GetComponent<Text>().text = title;
        //sticker.transform.GetChild(1).GetComponent<Text>().text = stickers[title].Description;
        //sticker.transform.GetChild(2).GetComponent<Text>().text = stickers[title].Points.ToString();
        sticker.transform.GetChild(4).GetComponent<Image>().sprite = stickers[title].Completed;
    }

    public void ChangeCategory(GameObject button)
    {
        StickerButton stickerButton = button.GetComponent<StickerButton>();

        scrollRect.content = stickerButton.categoryList.GetComponent<RectTransform>();

        stickerButton.OnClick();
        activeButton.OnClick();
        activeButton = stickerButton;
    }
}