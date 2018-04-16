using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ** Written by Hannes Gustafsson ** //

public class CorruptionVisuals : MonoBehaviour {

    private AudioManager audioManager;

    private Transform timelineObject;
    private Image maskImage;
    private Color originalColor;
    private RectTransform timelineRectTransform;
    private RectTransform corruptedAreaRectTransform;


    [Tooltip("Converts milliseconds to pixels, ie. how many milliseconds one pixel represents")] private float pixelToMs;
    [Tooltip("The original corrupted area rect width")] private float originalCorruptedRectWidth;
    [Tooltip("Start of the corruption in milliseconds")][SerializeField] private float corruptionStart;
    [Tooltip("End of the corruption in milliseconds")] [SerializeField] private float corruptionEnd;
    private float rectWidth;
    private float rectHeight;
    private int trackLength;

    // *** TODO ***
    // Connect the values 'corruptionStart' and 'corruptionEnd' to William's corruption script. Currently they are just values not connected to anything.

    void Start ()
    {

        // Initialize variables
        timelineObject = gameObject.transform.parent;
        maskImage = GetComponentInChildren<Image>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        timelineRectTransform = timelineObject.GetComponent<RectTransform>();
        corruptedAreaRectTransform = gameObject.GetComponent<RectTransform>();
        originalCorruptedRectWidth = corruptedAreaRectTransform.rect.width;
        trackLength = (int)audioManager.GetTrackLength();

        // Make sure the game object has a parent
        Debug.Assert(timelineObject != null, "Attach the corruption visual prefab as a child of Timeline Slider");


        // Calculate how many milliseconds one pixel represents
        rectWidth = timelineRectTransform.rect.width;
        pixelToMs = trackLength / rectWidth;
        Debug.Log("One pixel represents " + pixelToMs + " milliseconds");

        // Convert the start/endpoints to pixel values
        corruptionStart = corruptionStart / pixelToMs;
        corruptionEnd = corruptionEnd / pixelToMs;

        // Change the width of the corrupted area according to the pixel-to-ms values
        corruptedAreaRectTransform.sizeDelta = new Vector2(corruptionEnd - corruptionStart, timelineRectTransform.rect.height);
        
        /* 1. Place the anchor (middle) of the corrupted object at the start of the timeline position
         * 2. Place the start of the corrupted object at the start of the timeline position
         * 3. Place the start of the corrupted object at the start of the corruptionStart variable  */
        corruptedAreaRectTransform.localPosition = new Vector3(corruptedAreaRectTransform.localPosition.x - (originalCorruptedRectWidth / 2), 0, 0);
        corruptedAreaRectTransform.localPosition = new Vector3(corruptedAreaRectTransform.localPosition.x + corruptedAreaRectTransform.rect.width / 2, 0, 0);
        corruptedAreaRectTransform.localPosition = new Vector3(corruptedAreaRectTransform.localPosition.x + corruptionStart, 0, 0);

        // Save the original color to be able to reset it when the corrupted area has been fixed
        originalColor = maskImage.color;

	}


}
