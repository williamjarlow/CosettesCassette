using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerBook : MonoBehaviour {
    
    [SerializeField] private Image[] images;
    [SerializeField] private Text[] titles;
    [SerializeField] private Text[] descriptions;
    private List<Sprite> stickerSprites = new List<Sprite>();
    private List<string> stickerTitles = new List<string>();
    private List<string> stickerDescriptions = new List<string>();
    private StickerManager stickerManRef;
    
    void Start ()
    {

    }

    public void InitializeStickerPage()
    {
        stickerManRef = SaveSystem.Instance.transform.GetChild(0).GetComponent<StickerManager>();
        Debug.Log(stickerManRef.stickers.Length);
        for (int i = 0; i < stickerManRef.stickers.Length; i++)
        {
            Debug.Log("in for");
            //stickerPages.stickerSprites.Add(stickerManRef.stickers[i].Sprite);
            stickerSprites.Add(stickerManRef.stickers[i].Sprite);
            stickerTitles.Add(stickerManRef.stickers[i].Name);
            stickerDescriptions.Add(stickerManRef.stickers[i].Description);
        }

        for (int i = 0; i < 6; i++)
        {
            Debug.Log(images.Length + " , " + stickerSprites.Count);
            //images[i].GetComponent<Image>().sprite = stickerPages.stickerSprites[i];
            images[i].GetComponent<Image>().sprite = stickerSprites[i];
            titles[i].text = stickerTitles[i];
            descriptions[i].text = stickerDescriptions[i];
        }
    }

}
