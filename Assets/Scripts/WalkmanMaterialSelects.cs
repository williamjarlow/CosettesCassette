using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalkmanMaterialSelects : MonoBehaviour {

    [SerializeField] private GameObject walkmanRef;
    [SerializeField] private Texture textureRef;
    [SerializeField] private Texture buttonTextureRef;

    private UnityEngine.UI.Toggle toggle;

    // Use this for initialization
    void Start()
    {
        toggle = GetComponent<UnityEngine.UI.Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Select()
    {
        Renderer[] ass = walkmanRef.GetComponentsInChildren<Renderer>();
        ButtonController[] buttonRefs = walkmanRef.GetComponentsInChildren<ButtonController>();
        for (int i = 0; i < ass.Length; i++)
        {
            ass[i].material.mainTexture = this.textureRef;
        }

        for (int i = 0; i < buttonRefs.Length; i++)
        {
            buttonRefs[i].unselectedMaterial.mainTexture = this.textureRef;
        }

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
