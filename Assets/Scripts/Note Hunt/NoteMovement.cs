using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
    Standard,
    Arc,
    Glitchy
};

public class NoteMovement : MonoBehaviour {

    [HideInInspector] public int hitsRemaining;
    [HideInInspector] public MovementType movementType;

    private float RNGBounceTimer;
    [SerializeField] private float leftEdge;
    [SerializeField] private float rightEdge;

    [Header ("RNG bounce time range in seconds")]
    [SerializeField] private float RNGBounceLowerBound;
    [SerializeField] private float RNGBounceUpperBound;
    [SerializeField] private float speedx;
    [SerializeField] private float RNGSpeedxRange;
    [SerializeField] private float RNGSpeedyRange;
    [SerializeField] float upwardForce;
    [SerializeField] float downwardAcceleration;
    [SerializeField] float terminalVelocity;
    float spriteWidth;
    private bool headingLeft = true;
    [HideInInspector] public float speed;
    [HideInInspector] public int points;

    float gradualDecrease = 0;

	// Use this for initialization
	void Start ()
    {
        spriteWidth = GetComponent<SpriteRenderer>().sprite.bounds.extents.x;
        RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound);
        if (Random.Range(0, 2) == 0)
            headingLeft = false;
        speedx *= Random.Range((1- RNGSpeedxRange), (1 + RNGSpeedxRange));
        speed *= Random.Range((1 - RNGSpeedyRange), (1 + RNGSpeedyRange));
    }

    // Update is called once per frame
    void Update()
    {
        if (movementType == MovementType.Standard)
        {
            transform.Translate(Vector3.up * speed);
            RNGBounceTimer -= Time.deltaTime;
            if (RNGBounceTimer <= 0)
            {
                headingLeft = !headingLeft;
                RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound);
            }
            if (headingLeft == true)
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
        else if (movementType == MovementType.Arc)
        {
            gradualDecrease = Mathf.Clamp(gradualDecrease - downwardAcceleration, -terminalVelocity, 0); //Prevent wierd acceleration issues from occurring, just in case
            transform.Translate(Vector3.up * ((speed * 10) + gradualDecrease));
            if (headingLeft == true)
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
        else if (movementType == MovementType.Glitchy)
        {

        }
    }
}
