using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerButton : MonoBehaviour {

    //Accessed from StickerManager
    public GameObject categoryList;

    [SerializeField] private Sprite neutral, highlight;

    private Image sprite;

    // Use this for initialization
    void Awake () {
	}
	
	// Update is called once per frame
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

