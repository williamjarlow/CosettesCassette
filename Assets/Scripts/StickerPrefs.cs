using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerPrefs : MonoBehaviour {

    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private bool unlocked;
    [SerializeField] private int points;
    [SerializeField] private Sprite notCompleted;
    [SerializeField] private Sprite completed;
    [SerializeField] private GameObject stickerRef;
    [SerializeField] private StickerManager managerRef;

    private Dictionary<string, Sticker> stickersDictionary = new Dictionary<string, Sticker>();
    private StickerManager stickman;


    // Use this for initialization
    void Awake () {
        Sticker newSticker = new Sticker(name, description, points, stickerRef, notCompleted, completed);
        //stickersDictionary = managerRef.GetComponent<StickerManager>().stickers;

        /*stickersDictionary.Add(name, newSticker);
        managerRef.GetComponent<StickerManager>().stickers.Add(name, newSticker);*/
        managerRef.GetComponent<StickerManager>().stickers.Add(name, newSticker);
        /*stickman = managerRef.GetComponent<StickerManager>();
        stickman.stickers.Add(name, newSticker);*/
    }
	
	// Update is called once per frame
	void Update () {

	}
}

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
        set { stickerUnlocked = value; }
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



    public Sticker(string name, string description, int points, GameObject stickerRef, Sprite notCompleted, Sprite completed)
    {
        this.stickerName = name;
        this.stickerDescription = description;
        this.stickerUnlocked = false;
        this.stickerPoints = points;
        this.stickerRef = stickerRef;
        this.stickerNotCompleted = notCompleted;
        this.stickerCompleted = completed;
    }

    public bool EarnSticker()
    {
        if (!stickerUnlocked)
        {
            stickerUnlocked = true;
            return true;
        }
        return false;
    }
}
