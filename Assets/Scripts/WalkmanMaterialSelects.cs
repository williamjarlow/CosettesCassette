using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalkmanMaterialSelects : MonoBehaviour {

	private AudioManager audioManager;
    [Header("Walkman Color changer")]
	[SerializeField] private GameObject walkmanRef;
    [SerializeField] private Texture textureRef;

    [Header("Settings Icon changer")]
    [SerializeField] private GameObject settingsIcon;
    [SerializeField] private Texture buttonTextureRef;

    [Header("PressedButtonsMaterial")]
    [SerializeField] private Material buttonsmaterial;
    [SerializeField] private Texture shadedTextureRef;

    [Header("Gradientbar")]
    [SerializeField] public GameObject corruptionBarMat;
    [SerializeField] private Sprite corrutionBarTex;
    
    private UnityEngine.UI.Toggle toggle;

    // Use this for initialization
    void Start()
    {
		audioManager = GameObject.FindGameObjectWithTag ("AudioManager").GetComponent<AudioManager>();
        //toggle = GetComponent<UnityEngine.UI.Toggle>();
        //toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Select()
    {
        //walkmanRef.GetComponent<Renderer>().sharedMaterials[0].SetTexture("_MainTex", textureRef);
        
        Renderer[] ass = walkmanRef.GetComponentsInChildren<Renderer>();
        ButtonController[] buttonRefs = walkmanRef.GetComponentsInChildren<ButtonController>();
        for (int i = 0; i < ass.Length; i++)
        {
            if (ass[i].tag != "Cassette")
            {

                ass[i].sharedMaterials[0].SetTexture("_MainTex", textureRef);
                
            }
            
        }

        settingsIcon.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", buttonTextureRef);
        buttonsmaterial.SetTexture("_MainTex", shadedTextureRef);
        //corruptionBarMat.GetComponent<Image>().overrideSprite = corrutionBarTex;
        audioManager.PlaySkinMenuSelect ();

        /*
        for (int i = 0; i < buttonRefs.Length; i++)
        {
            buttonRefs[i].unselectedMaterial.mainTexture = this.textureRef;
        }
        */
        //walkmanRef.GetComponent<Renderer>().material.mainTexture = textureRef;
        //walkmanLidRef.GetComponent<Renderer>().materials[1].mainTexture = textureRef;
    }
    private void OnToggleValueChanged(bool isOn)
    {
        ColorBlock cb = toggle.colors;
        if (isOn)
        {
            cb.normalColor = Color.yellow;
            cb.highlightedColor = Color.yellow;
        }
        else if (!isOn)
        {
            cb.normalColor = Color.white;
            cb.highlightedColor = Color.white;
        }
        toggle.colors = cb;
    }
}
