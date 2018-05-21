using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_MeshColorChanger : MonoBehaviour {

    // Used to change the color of the buttons when enabled/disabled
    [HideInInspector] public MeshRenderer meshRenderer;
    private Material material;
    public Material originalMaterial;
    public Material disabledMaterial;

    void Start ()
    {
        //meshRenderer = GetComponent<MeshRenderer>();
        material = GetComponent<MeshRenderer>().material;

	}
	
	void Update ()
    {
        SetDisabledMaterial();

        Debug.Log(GetComponent<MeshRenderer>().material);
    }

    public void SetDisabledMaterial()
    {
        //material = disabledMaterial;
        GetComponent<MeshRenderer>().material = disabledMaterial;
    }

    public void SetEnabledMaterial()
    {
        material = originalMaterial;
    }
}
