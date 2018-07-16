using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ** Written by Hannes Gustafsson ** //

public class CorruptionVisuals : MonoBehaviour {

    private AudioManager audioManager;

    [SerializeField] private Image maskImage;
    private Color originalColor;
    public Color repairedColor;
    private RectTransform timelineRectTransform;
    private RectTransform corruptedAreaRectTransform;
    private bool fade = false;

    GameManager gameManager;

    [Tooltip("Converts milliseconds to pixels, ie. how many milliseconds one pixel represents")] private float pixelToMs;
    [Tooltip("The original corrupted area rect width")] private float originalCorruptedRectWidth;
    [Tooltip("Start of the corruption in milliseconds")] private float corruptionStart;
    [Tooltip("End of the corruption in milliseconds")] private float corruptionEnd;
    private float rectWidth;
    private float rectHeight;
    private int trackLength;
    [SerializeField] private float fadeDelay = 3.45f;
    [SerializeField] private float fadeSpeed = 0.4f;
    Color savedColor;
    private float corruptedPercentage = 100;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Start ()
    {
        // Save the original color to be able to reset it when the corrupted area has been fixed
        //maskImage = GetComponentInChildren<Image>();
        repairedColor = gameManager.timelineSlider.GetComponent<Image>().color;
        savedColor = maskImage.color;
        savedColor.a = 0;
        StartCoroutine(FadeInDelay(fadeDelay));
    }

    void Update()
    {
        if (fade)
        {
            savedColor.a += Time.deltaTime * fadeSpeed;
            maskImage.color = savedColor;
            if (maskImage.color.a >= 1 - corruptedPercentage)
                fade = false;
        }
    }

    public void SetCorruptionPosition(int corruptionStartPoint, int corruptionEndPoint)
    {
        // Initialize variables
        audioManager = gameManager.audioManager;
        corruptedAreaRectTransform = gameObject.GetComponent<RectTransform>();
        originalCorruptedRectWidth = corruptedAreaRectTransform.rect.width;
        timelineRectTransform = gameObject.transform.parent.GetComponent<RectTransform>();
        trackLength = (int)audioManager.GetTrackLength();

        // Calculate how many milliseconds one pixel represents
        rectWidth = timelineRectTransform.rect.width;
        pixelToMs = trackLength / rectWidth;

        // Convert start- and endpoint values to pixel values
        corruptionStart = corruptionStartPoint / pixelToMs;
        corruptionEnd = corruptionEndPoint / pixelToMs;

        // Change the width of the corrupted area according to the pixel-to-ms values
        corruptedAreaRectTransform.sizeDelta = new Vector2(corruptionEnd - corruptionStart, timelineRectTransform.rect.height);
        
        /* 1. Place the anchor (middle) of the corrupted object at the start of the timeline position
         * 2. Place the start of the corrupted object at the start of the timeline position
         * 3. Place the start of the corrupted object at the start of the corruptionStart variable  */
        corruptedAreaRectTransform.localPosition = new Vector3(corruptedAreaRectTransform.localPosition.x - (originalCorruptedRectWidth / 2), 0, 0);
        corruptedAreaRectTransform.localPosition = new Vector3(corruptedAreaRectTransform.localPosition.x + corruptedAreaRectTransform.rect.width / 2, 0, 0);
        corruptedAreaRectTransform.localPosition = new Vector3(corruptedAreaRectTransform.localPosition.x + corruptionStart, 0, 0);

    }

    public void RestoreOriginalColor()
    {
        maskImage.color = repairedColor;
    }

    public void SetAlpha(float corruptionPercentage)
    {
        // Convert percentage to alpha values
        corruptionPercentage /= 100;

        // Save previous color values
        Color savedColor = maskImage.color;

        // Edit alpha. 1 == Fully visible, 0 == fully transparent
        savedColor.a = 1 - corruptionPercentage;

        // Set alpha value to actual image
        maskImage.color = savedColor;

        // Get value to use for fade in
        corruptedPercentage = corruptionPercentage;
    }

    IEnumerator FadeInDelay(float fadeDelay)
    {
        yield return new WaitForSeconds(fadeDelay);
        fade = true;
    }
}
