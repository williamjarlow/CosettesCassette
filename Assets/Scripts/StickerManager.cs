using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerManager : MonoBehaviour {

    [SerializeField] private GameObject saveSystemRef;
    [SerializeField] private GameObject stickerMenuRef;
    public Sticker[] stickers;

    public Dictionary<string, Sticker> stickersDic = new Dictionary<string, Sticker>();

    void Start ()
    {
        for (int i=0; i < stickers.Length; i++)
        {
            stickersDic.Add(stickers[i].Name, stickers[i]);
        }

        saveSystemRef.GetComponent<SaveSystem>().LoadStickers(stickersDic);
    }
	
	void Update () {

        if (Input.GetKeyDown(KeyCode.V))
        {
            saveSystemRef.GetComponent<SaveSystem>().ClearStickers(stickersDic);
        }
    }

    //Earns the chosen sticker, call thiss with the exact stickers name to earn the sticker and set its correct values in the sticker menu
    public bool EarnSticker(string title)
    {
        if(stickersDic[title].EarnSticker())
        {
            saveSystemRef.GetComponent<SaveSystem>().SaveStickers(title, true);
            return true;
        }
        return false;
    }
}
