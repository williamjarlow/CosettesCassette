using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Zoom : MonoBehaviour {

    [SerializeField] private Image originalPic;
    [SerializeField] private Vector3 originalSize;
    [SerializeField] private Vector3 zoomSize;
    [SerializeField] private Vector3 zoomed;
    [SerializeField] private float increaseSize;


    void Start()
    {
        
        originalSize = transform.localScale;
        zoomSize = originalSize;

    }


    public void Zooming()
    {
        if (transform.localScale == originalSize)
        {
            zoomed = zoomSize * increaseSize;
            transform.localScale = zoomed;
        }
        else if (transform.localScale == zoomed)
        {
            transform.localScale = originalSize;
        }
    }

    public void OnMouseExit()
    {


    }

}
