﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageAnimation : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Color currentColor;

    [Header("To have image blink with alpha")]
    [SerializeField] private bool blinkWithAlpha = false;
    [SerializeField] private float minAlpha = 0.6f;
    [SerializeField] private float maxAlpha = 1;
    [SerializeField] private float blinkingSpeed = 1;

    [Header("To rotate image back and forth")]
    [SerializeField] private bool rotateImage = false;
    [SerializeField] private float maxRotation = 30;
    [SerializeField] private float rotationSpeed = 30;

    void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentColor = GetComponent<SpriteRenderer>().color;
	}
	

	void Update ()
    {
        if (blinkWithAlpha)
        {
            currentColor.a = Mathf.Lerp(maxAlpha, minAlpha, Mathf.PingPong(Time.time * blinkingSpeed, 1));
            spriteRenderer.color = currentColor;
        }

        if (rotateImage)
            transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * rotationSpeed, maxRotation) - maxRotation/2);
    }
}
