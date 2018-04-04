using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HenkTestTouch : MonoBehaviour {

    private AudioSource cassetteSong;

    // The position where the player first placed his finger...
    // Starts outside of screen with a negative Vector2.one
    private Vector2 touchOrigin = -Vector2.one;

    private float horizontal = 0;     //Used to store the horizontal move direction.
    private float vertical = 0;       //Used to store the vertical move direction.
    [Tooltip("This is the minimum amount of movement needed to trigger that we actually moved at all.")]
    [SerializeField] private float minAmountOfMovementToTrigger;
    //private Vector3 accel = Vector3.zero;   // Phones accelerometer values

    //[SerializeField] private float speed;
    public ProgressBar progressScript;
    private GameManager gameManager;

    private void Awake()
    {
        cassetteSong = GetComponent<AudioSource>();
        gameManager = GetComponent<GameManager>();

    }
    void Start ()
    {
        //cassetteSong.time = 53;     // Where to start in song, 'cause Backstreet Boys!
	}
	
	void Update ()
    {
        TouchControls();
        MoveSong();
    }


    /*
    Vector3 SwipeSpeed()
    {
        // We assume that the device is held parallel to the ground...
        // and the Home button is in the right hand

        // Remap the device acceleration axis to game coordinates:
        // 1) XY plane of the device is mapped onto XZ plane
        // 2) Rotated 90 degrees around Y axis
        accel.x = -Input.acceleration.y;
        accel.y = Input.acceleration.x;

        // Clamp acceleration vector to the unit sphere
        if (accel.sqrMagnitude > 1)
            accel.Normalize();

        // Make it move 10 meters per second instead of 10 meters per frame
        accel *= Time.deltaTime;

        return accel;
    }
    */

    void MoveSong()
    {

        if (horizontal < 0)
            cassetteSong.pitch = horizontal;

        if (horizontal > 0)
            cassetteSong.pitch = horizontal;

        else if (horizontal == 0)
            cassetteSong.pitch = 1;
    }


    void TouchControls()
    {
        if (Input.touchCount > 0)       // Registered a touch
        {
            Touch myTouch = Input.touches[0];   // Grab first touch

            if (myTouch.phase == TouchPhase.Began)  // Determine if we're at the beginning of a touch
            {
                touchOrigin = myTouch.position;     // Set origin at startpos of finger.
            }

            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)   // Did touch end and is inside of screen?
            {

                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x; // Get difference between beginning and end of touch in x-axis
                float y = touchEnd.y - touchOrigin.y; // Get difference between beginning and end of touch in y-axis


                // Move song progress if we touch inside of bar to that specific touch location
                // Make sure we are inside of bars boundaries in X and Y
                if ((touchEnd.x >= progressScript.barStartPositionX && touchEnd.x <= progressScript.barEndPositionX) && (touchEnd.y >= progressScript.barStartPositionY && touchEnd.y <= progressScript.barEndPositionY))
                {
                    float correctPosOnBar = (touchEnd.x - progressScript.barStartPositionX);   // Correct position on bar is current position of finger - blank space before bar.                    
                    float songPercentValue = (correctPosOnBar / progressScript.barLength);     // Get percentage amount of progress into song by taking percentage amount of bar.
                    cassetteSong.time = (gameManager.lengthOfSong * songPercentValue);         // Use percentage to move to correct place in song.
                } 





                if ((touchEnd.x - touchOrigin.x <= -minAmountOfMovementToTrigger) || (touchEnd.x - touchOrigin.x >= minAmountOfMovementToTrigger))    // Did finger move less than 2 of whatever they count in...
                {

                    horizontal = Mathf.Clamp(x * 0.01f, -3, 3);     // Get lower sensitivy, hence magic value of 0.01. Then clamp it to -3 and 3 as that's the span for pitch.
                    vertical = Mathf.Clamp(y * 0.01f, -3, 3);

                }
                else {
                    horizontal = 0;
                    vertical = 0;
                }

                touchOrigin.x = -1;     // Put origin outside of screen to not trigger anything

            }
        }
    }


}
