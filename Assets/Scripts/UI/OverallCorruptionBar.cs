using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//** Written by Hannes Gustafsson **//

public class OverallCorruptionBar : MonoBehaviour {

    [HideInInspector] public Image thisImage;
    private OverallCorruption overallCorruption;
    GameManager gameManager;
    private float corruptionPercentage;
    private float startValue;
    [SerializeField] private float speed = 1;
    [SerializeField] private float clearLineWidth = 7;

    private void Awake()
    {
    }

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        thisImage = GetComponent<Image>();
        overallCorruption = gameManager.overallCorruption;

        thisImage.fillAmount = 0;
        AddLine();
    }

    void Update()
    {
        corruptionPercentage = overallCorruption.overallCorruption;

        // Divide by 100 because fillAmount takes values from 0 --> 1 and overallCorrption has values from 0 --> 100
        thisImage.fillAmount = Mathf.Lerp(thisImage.fillAmount, 1 - overallCorruption.overallCorruption / 100, Time.deltaTime * speed);
    }

    private void AddLine()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.useWorldSpace = false;
        lineRenderer.widthMultiplier = 0.25f;
        lineRenderer.positionCount = 2;
        float barLength = thisImage.rectTransform.sizeDelta.x;
        Vector3 linePosition = new Vector3(thisImage.rectTransform.position.x - (barLength - (barLength / 2)) + (barLength * ((float)overallCorruption.corruptionClearThreshold / 100)), thisImage.rectTransform.position.y, 0);
        lineRenderer.SetPosition(0, linePosition);
        lineRenderer.SetPosition(1, linePosition + new Vector3(clearLineWidth, 0, 0));
    }
}
