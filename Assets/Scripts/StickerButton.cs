using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerButton : MonoBehaviour {

    //Accessed from StickerManager
    public GameObject categoryList;

    [SerializeField] private Sprite neutral, highlight;

    private Image sprite;

    void Awake () {
	}

    //Simply switch the visual A E S T H E T I C S of the category button.
    public void OnClick ()
    {
        sprite = GetComponent<Image>();
        if (sprite.sprite == neutral)
        {
            sprite.sprite = highlight;
            categoryList.SetActive(true);
        }
        else
        {
            sprite.sprite = neutral;
            categoryList.SetActive(false);
        }
	}
}

