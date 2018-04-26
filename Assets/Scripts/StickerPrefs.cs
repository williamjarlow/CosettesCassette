using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StickerPrefs : MonoBehaviour {

    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private int points;
    [SerializeField] private Sprite notCompleted;
    [SerializeField] private Sprite completed;
    [SerializeField] private GameObject stickerRef;
    [SerializeField] private Image imageRef;
    [SerializeField] private StickerManager managerRef;
    


    // Use this for initialization
    void Awake () {
        Sticker newSticker = new Sticker(name, description, points, stickerRef, imageRef, notCompleted, completed);

        //This line HAS TO be run BEFORE the sticker menu is disabled in StickerManager script
        //All stickers are a part of their category in the sticker menu, if they dont get instantiated before the
        //sticker menu is disabled the stickers wont get added to the stickers dictionary in StickerManager, 
        //this pretty much fucks everything up so dont do this
        managerRef.GetComponent<StickerManager>().stickers.Add(name, newSticker);
    }
}

[Serializable]
public class Sticker
{
    private string stickerName;

    public string Name
    {
        get { return stickerName; }
        set { stickerName = value; }
    }

    private string stickerDescription;

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

    private int stickerPoints;

    public int Points
    {
        get { return stickerPoints; }
        set { stickerPoints = value; }
    }

    private Sprite stickerNotCompleted;
    private Sprite stickerCompleted;

    public Sprite Completed
    {
        get { return stickerCompleted; }
        set { stickerCompleted = value; }
    }

    private GameObject stickerRef;
    private Image imageRef;

    //Accessed from save system
    public bool loaded;

    //Sticker constructor
    public Sticker(string name, string description, int points, GameObject stickerRef, Image imageRef, Sprite notCompleted, Sprite completed)
    {
        this.stickerName = name;
        this.stickerDescription = description;
        this.stickerUnlocked = false;
        this.stickerPoints = points;
        this.stickerRef = stickerRef;
        this.imageRef = imageRef;
        this.stickerNotCompleted = notCompleted;
        this.stickerCompleted = completed;
    }

    //Earning stickers requires us to change the sprite in image to show that we have earned it
    public bool EarnSticker()
    {
        if (!stickerUnlocked)
        {
            this.imageRef.GetComponent<Image>().sprite = this.stickerCompleted;
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
            this.imageRef.GetComponent<Image>().sprite = this.stickerCompleted;
            stickerUnlocked = true;
        }
        else if (this.loaded == false)
        {
            this.imageRef.GetComponent<Image>().sprite = this.stickerNotCompleted;
            stickerUnlocked = false;
        }
    }
}
