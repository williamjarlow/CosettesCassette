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

    GameManager gameManager;

    [Header ("RNG bounce time range in seconds")]
    [SerializeField] private float RNGBounceLowerBound;
    [SerializeField] private float RNGBounceUpperBound;
    [SerializeField] private float speedx;
    [SerializeField] private float RNGSpeedxRange;
    [SerializeField] private float RNGSpeedyRange;

    [Header("Arc note variables")]
    [Tooltip("Initial upward momentum")]
    [Range(0.01f, 0.2f)] [SerializeField] float upwardForce;
    [Tooltip("Strength of gravity")]
    [Range(0, 0.1f)][SerializeField] float downwardAcceleration;
    [Tooltip("Maximum downward speed")]
    [Range(0.01f, 1f)][SerializeField] float terminalVelocity;

    [Header("Glitch note variables")]
    [Tooltip("Time between each teleport, measured in beats for that crisp, clutch feel")]
    [Range(1, 32)] [SerializeField] int teleportDelay;

    [Tooltip("Distance of each individual teleport. WARNING: If this number is set too high, the mechanic might not work at all. Use with caution.")]
    [Range(1, 30)] [SerializeField] int teleportDistance;

    float spriteWidth;
    private bool headingLeft = true;
    bool headingUp = true;
    [HideInInspector] public float speed;
    [HideInInspector] public int points;

    float bounceTimer = 0;
    float gradualDecrease = 0;

	// Use this for initialization
	void Start ()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        spriteWidth = GetComponent<SpriteRenderer>().sprite.bounds.extents.x;
        RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound);
        if (Random.Range(0, 2) == 0) //Randomize starting direction
            headingLeft = false;
        speedx *= Random.Range((1- RNGSpeedxRange), (1 + RNGSpeedxRange)); //Randomize xSpeed
        speed *= Random.Range((1 - RNGSpeedyRange), (1 + RNGSpeedyRange)); //Randomize ySpeed
    }

    // Update is called once per frame
    void Update()
    {
        if (movementType == MovementType.Standard) //Standard movement
        {
            transform.Translate(Vector3.up * speed); //Move note upward with a constant speed
            RNGBounceTimer -= Time.deltaTime; //Decrement the random bounce timer.
            if (RNGBounceTimer <= 0) //If the random bounce timer hits zero...
            {
                headingLeft = !headingLeft; //...change the direction of the note...
                RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound); //...and reset the random bounce timer.
            }

            if (headingLeft == true)
            {
                transform.Translate(Vector3.left * speedx);
                if (transform.position.x <= leftEdge + spriteWidth) //If wall has been hit
                {
                    headingLeft = false;
                    RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound);
                }
            }
            else //If heading right
            {
                transform.Translate(Vector3.right * speedx);
                if (transform.position.x >= rightEdge - spriteWidth) //If wall has been hit
                {
                    headingLeft = true;
                    RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound); 
                }
            }
        }
        else if (movementType == MovementType.Arc) //Arcing movement
        {
            gradualDecrease -= downwardAcceleration; //Move note upward with an initial force and let gravity affect note
            transform.Translate(Vector3.up * Mathf.Clamp(upwardForce + gradualDecrease, -terminalVelocity, upwardForce));
            if (headingLeft == true)
            {
                transform.Translate(Vector3.left * speedx);
                if (transform.position.x <= leftEdge + spriteWidth) //If wall has been hit
                {
                    headingLeft = false;
                    RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound);
                }
            }
            else //If heading right
            {
                transform.Translate(Vector3.right * speedx);
                if (transform.position.x >= rightEdge - spriteWidth) //If wall has been hit
                {
                    headingLeft = true;
                    RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound);
                }
            }
        }
        else if (movementType == MovementType.Glitchy) //Standard movement but with occasional downward teleports
        {   
            RNGBounceTimer -= Time.deltaTime;
            bounceTimer -= Time.deltaTime; //Bounce timer is used to determine when a downwarp should occur
            if (bounceTimer <= 0) //If the note should downwarped
            {
                transform.Translate(Vector3.down * speed * teleportDistance);  //Downwarp the note
                bounceTimer = (gameManager.overallCorruption.bpmInMs*teleportDelay/1000); //Restart the bounce timer
            }

            transform.Translate(Vector3.up * speed);

            if (RNGBounceTimer <= 0)
            {
                headingLeft = !headingLeft;
                RNGBounceTimer = Random.Range(RNGBounceLowerBound/3, RNGBounceUpperBound/3);
            }

            if (headingLeft == true)
            {
                transform.Translate(Vector3.left * speedx);
                if (transform.position.x <= leftEdge + spriteWidth) //If wall has been hit
                {
                    headingLeft = false;
                    RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound);
                }
            }
            else //If heading right
            {
                transform.Translate(Vector3.right * speedx);
                if (transform.position.x >= rightEdge - spriteWidth) //If wall has been hit
                {
                    headingLeft = true;
                    RNGBounceTimer = Random.Range(RNGBounceLowerBound, RNGBounceUpperBound);
                }
            }
        }
    }
}
