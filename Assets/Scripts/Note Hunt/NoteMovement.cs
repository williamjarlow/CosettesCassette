using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMovement : MonoBehaviour {


    private float RNGBounceTimer;
    [SerializeField] private float leftEdge;
    [SerializeField] private float rightEdge;

    [Header ("RNG bounce time range in seconds")]
    [SerializeField] private float RNGBounceLowerBound;
    [SerializeField] private float RNGBounceUpperBound;
    [SerializeField] private float speedx;
    float spriteWidth;
    private bool headingLeft = true;
    [HideInInspector] public float speed;
    [HideInInspector] public int points;
	// Use this for initialization
	void Start ()
    {
        spriteWidth = GetComponent<SpriteRenderer>().sprite.bounds.extents.x;
        RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound);
        if (Random.Range(0, 2) == 0)
            headingLeft = false;
    }

    // Update is called once per frame
    void Update ()
    {
        transform.Translate(Vector3.up * speed);
        RNGBounceTimer -= Time.deltaTime;

        if(RNGBounceTimer <= 0)
        {
            headingLeft = !headingLeft;
            RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound);
        }
        if (headingLeft == true )
        {
            transform.Translate(Vector3.left * speedx);
            if (transform.position.x <= leftEdge + spriteWidth)
            {
                headingLeft = false;
                RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound);
            }
        }
        else
        {
            transform.Translate(Vector3.right * speedx);
            if (transform.position.x >= rightEdge - spriteWidth)
            {
                headingLeft = true;
                RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound);
            }
        }

    }

}
