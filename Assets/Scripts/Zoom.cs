using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Zoom : MonoBehaviour {

    [SerializeField] private Image originalPic;
    private Vector3 originalSize;
    private Vector3 originalPos;
    private Vector3 zoomSize;
    [SerializeField] private Vector3 zoomPos;
    private Vector3 zoomed;
    [SerializeField] private float increaseSize;


    void Start()
    {
        
        originalSize = transform.localScale;
        originalPos = transform.localPosition;
        zoomSize = originalSize;

    }


    public void Zooming()
    {
        if (transform.localScale == originalSize)
        {
            zoomed = zoomSize * increaseSize;
            transform.localPosition = zoomPos;
            transform.localScale = zoomed;
        }
        else if (transform.localScale == zoomed)
        {
            transform.localPosition = originalPos;
            transform.localScale = originalSize;
        }
    }

    public void OnMouseExit()
    {


    }

}
