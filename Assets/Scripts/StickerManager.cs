using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerManager : MonoBehaviour {

    private StickerButton activeButton;
    //[SerializeField] private ScrollRect scrollRect;
    //[SerializeField] private GameObject generalBtn;
    //[SerializeField] private GameObject stickerMenu;
    //[SerializeField] private GameObject[] otherCategories;
    //[SerializeField] private GameObject particleSticker;
    [SerializeField] private GameObject saveSystemRef;
    //[SerializeField] private int showDuration;

    //[SerializeField] private GameObject stickerPrefab;
    //private bool menuDisable = true;

    public Dictionary<string, Sticker> stickers = new Dictionary<string, Sticker>();

    void Start ()
    {

        //activeButton = generalBtn.GetComponent<StickerButton>();
        //activeButton.OnClick();

        ////Deactivate all other categories than the chosen start category, this needs to be done as the stickers
        ////in each category HAS TO be instantiated before we load the stickers.
        //for(int i=0; i < otherCategories.Length; i++)
        //{
        //    otherCategories[i].SetActive(false);
        //}

        saveSystemRef.GetComponent<SaveSystem>().LoadStickers(stickers);
    }
	
	void Update () {
        //This HAS TO be done AFTER the Awake() method in StickerPrefs or it wont work
        //This solution is temporary, if someone has a better idea feel free to change
        //if(menuDisable == true)
        //{
        //    menuDisable = false;
        //    stickerMenu.SetActive(false);
        //}

        //Example on how to earn a sticker
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EarnSticker("PressQ");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            EarnSticker("PressW");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            EarnSticker("PressE");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            saveSystemRef.GetComponent<SaveSystem>().ClearStickers(stickers);
        }
    }

    //Earns the chosen sticker, call thiss with the exact stickers name to earn the sticker and set its correct values in the sticker menu
    public bool EarnSticker(string title)
    {
        if(stickers[title].EarnSticker())
        {
            //GameObject sticker = particleSticker;
            //sticker.SetActive(true);
            //SetVisualSticker(sticker, title);
            //StartCoroutine(HideSticker(sticker));
            saveSystemRef.GetComponent<SaveSystem>().SaveStickers(title, true);
            return true;
        }
        return false;
    }

    ////Wait for some time and disable the sticker notification
    //public IEnumerator HideSticker(GameObject sticker)
    //{
    //    yield return new WaitForSeconds(showDuration);
    //    sticker.SetActive(false);
    //}

    ////Set the corresponding sticker values inside the visual element of earning a sticker.
    ////The GetChild is hardcoded to find the Image child of the Particle_sticker gameobject, find better solution?
    //public void SetVisualSticker(GameObject sticker, string title)
    //{
    //    Vector3 temp = sticker.transform.GetChild(4).GetComponent<Image>().rectTransform.localPosition;
    //    temp.x = 0;
    //    temp.y = 20;
    //    sticker.transform.GetChild(4).GetComponent<Image>().rectTransform.localPosition = temp;
    //    sticker.transform.GetChild(4).GetComponent<Image>().sprite = stickers[title].Completed;
    //}

    ////Change the category on button clicks in the sticker album
    //public void ChangeCategory(GameObject button)
    //{
    //    StickerButton stickerButton = button.GetComponent<StickerButton>();

    //    scrollRect.content = stickerButton.categoryList.GetComponent<RectTransform>();

    //    stickerButton.OnClick();
    //    activeButton.OnClick();
    //    activeButton = stickerButton;
    //}
}
