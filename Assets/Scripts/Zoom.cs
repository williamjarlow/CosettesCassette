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
    private bool isZoomed = false;


    void Start()
    {
        
        originalSize = transform.localScale;
        originalPos = transform.localPosition;
        zoomSize = originalSize;

    }


    public void Zooming()
    {
        if (isZoomed == false)
        {
            zoomed = zoomSize * increaseSize;
            transform.localPosition = zoomPos;
            transform.localScale = zoomed;
            isZoomed = true;
        }
        else if (isZoomed == true)
        {
            transform.localPosition = originalPos;
            transform.localScale = originalSize;
            isZoomed = false;
        }
    }

}
