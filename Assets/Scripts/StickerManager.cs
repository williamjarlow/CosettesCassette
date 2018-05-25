using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerManager : MonoBehaviour {
    
    [SerializeField] private GameObject saveSystemRef;
    public Sticker[] stickers;

    public Dictionary<string, Sticker> stickerDictionary = new Dictionary<string, Sticker>();

    void Start ()
    {
        for (int i = 0; i < stickers.Length; i++)
        {
            stickerDictionary.Add(stickers[i].Name, stickers[i]);
        }

        saveSystemRef.GetComponent<SaveSystem>().LoadStickers(stickerDictionary);
    }
	
	void Update () {

        if (Input.GetKeyDown(KeyCode.V))
        {
            saveSystemRef.GetComponent<SaveSystem>().ClearStickers(stickerDictionary);
        }
    }

    //Earns the chosen sticker, call thiss with the exact stickers name to earn the sticker and set its correct values in the sticker menu
    public bool EarnSticker(string title)
    {
        if(stickerDictionary[title].EarnSticker())
        {
            saveSystemRef.GetComponent<SaveSystem>().SaveStickers(title, true);
            return true;
        }
        return false;
    }
}
