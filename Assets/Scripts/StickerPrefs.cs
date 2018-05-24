using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StickerPrefs : MonoBehaviour {

    [SerializeField] private string title;
    [SerializeField] private string description;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject stickerRef;
    [SerializeField] private SpriteRenderer imageRef;
    [SerializeField] private GameObject saveSystemRef;
    
}

[Serializable]
public class Sticker
{
    public string stickerName;

    public string Name
    {
        get { return stickerName; }
        set { stickerName = value; }
    }

    public string stickerDescription;

    public string Description
    {
        get { return stickerDescription; }
        set { stickerDescription = value; }
    }

    private bool stickerUnlocked;

    public bool Unlocked
    {
        get { return stickerUnlocked; }
        set { stickerUnlocked = value; SetLoadedSticker(); }
    }

    //private GameObject stickerRef;
    private SpriteRenderer imageRef;
    public SpriteRenderer ImageRef
    {
        get { return imageRef; }
        set { imageRef = value; }
    }

    public Sprite Sprite
    {
        get { return sprite; }
        set { sprite = value; }
    }

    public Sprite sprite;

    //Accessed from save system
    [HideInInspector]public bool loaded;

    //Sticker constructor
    public Sticker(string name, string description, Sprite sprite)
    {
        this.stickerName = name;
        this.stickerDescription = description;
        this.stickerUnlocked = false;
        this.Sprite = sprite;
    }

    //Earning stickers requires us to change the sprite in image to show that we have earned it
    public bool EarnSticker()
    {
        if (!this.stickerUnlocked)
        {
            this.stickerUnlocked = true;
            this.loaded = true;
            return true;
        }
        return false;
    }

    //Load stickers, set the correct image sprite in sticker album
    public void SetLoadedSticker()
    {
        if (this.loaded == true)
        {
            stickerUnlocked = true;
        }
        else if (this.loaded == false)
        {
            stickerUnlocked = false;
        }
    }
}
