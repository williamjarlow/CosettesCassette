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

    //[SerializeField] private int points;
    //[SerializeField] private Sprite notCompleted;
    //[SerializeField] private Sprite completed;



    //// Use this for initialization
    //void Awake () {
    //    Sticker newSticker = new Sticker(title, description, stickerRef, imageRef);

    //    //This line HAS TO be run BEFORE the sticker menu is disabled in StickerManager script
    //    //All stickers are a part of their category in the sticker menu, if they dont get instantiated before the
    //    //sticker menu is disabled the stickers wont get added to the stickers dictionary in StickerManager, 
    //    //this pretty much fucks everything up so dont do this
    //    managerRef.GetComponent<StickerManager>().stickers.Add(title, newSticker);
    //}
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

    //private int stickerPoints;

    //public int Points
    //{
    //    get { return stickerPoints; }
    //    set { stickerPoints = value; }
    //}

    //private Sprite stickerNotCompleted;
    //private Sprite stickerCompleted;

    //public Sprite Completed
    //{
    //    get { return stickerCompleted; }
    //    set { stickerCompleted = value; }
    //}

    //Accessed from save system
    [HideInInspector]public bool loaded;

    //Sticker constructor
    public Sticker(string name, string description, Sprite sprite)
    {
        this.stickerName = name;
        this.stickerDescription = description;
        this.stickerUnlocked = false;
        this.Sprite = sprite;
        //this.stickerRef = stickerRef;
        //this.stickerPoints = points;
        //this.stickerNotCompleted = notCompleted;
        //this.stickerCompleted = completed;
    }

    //Earning stickers requires us to change the sprite in image to show that we have earned it
    public bool EarnSticker()
    {
        if (!stickerUnlocked)
        {
            //this.imageRef.GetComponent<Image>().sprite = this.stickerCompleted;
            //this.ImageRef.GetComponent<SpriteRenderer>().color = Color.white;
            stickerUnlocked = true;
            return true;
        }
        return false;
    }

    //Load stickers, set the correct image sprite in sticker album
    public void SetLoadedSticker()
    {
        if (this.loaded == true)
        {
            //this.imageRef.GetComponent<Image>().sprite = this.stickerCompleted;
            //this.ImageRef.GetComponent<SpriteRenderer>().color = Color.white;
           
            stickerUnlocked = true;
        }
        else if (this.loaded == false)
        {
            //this.imageRef.GetComponent<Image>().sprite = this.stickerNotCompleted;
            //this.ImageRef.GetComponent<SpriteRenderer>().color = Color.black;
            stickerUnlocked = false;
        }
    }
}
