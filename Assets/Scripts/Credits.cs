using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Credits : MonoBehaviour {

    [SerializeField] private float stopPos;
    [SerializeField] private float duration = 120;
    private GameManager gameManager;
    private AudioManager audioManager;
    private bool LerpStarted = false;

    // Use this for initialization
    void Start ()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        audioManager = gameManager.audioManager;
    }

    // Update is called once per frame
    void Update ()
    {
        if(LerpStarted == false)
        {
            LerpStarted = true;
            StartCoroutine(MoveFromTo(new Vector3(transform.localPosition.x, transform.localPosition.y + stopPos, transform.localPosition.z), transform.localPosition, duration));
        }
    }

    IEnumerator MoveFromTo(Vector3 pointA, Vector3 pointB, float time)
    {
        bool moving = false;
        if (!moving)
        {   
            moving = true;     
            float t = 1.0f;
            while (t >= 0.0f)
            {
                t -= Time.deltaTime / time; 
                transform.localPosition = Vector3.Lerp(pointA, pointB, t); // Set position proportional to t
                yield return new WaitForEndOfFrame();
            }
            moving = false;
        }
    }
}

