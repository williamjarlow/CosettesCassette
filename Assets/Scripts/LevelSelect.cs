using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class LevelSelect : MonoBehaviour
{

    private int currentFocus;

    private int cassetteAmount;

    [SerializeField] private int rotationAmount = 50;

    public List<RaycastObject> cassettes = new List<RaycastObject>();

    private Vector3 origPosition = Vector3.zero;

    [SerializeField] private float minimumMovementForChange = 5;

    // Temporary
    ////
    [Header("For testing purposes, activate before starting")]
    [SerializeField] private bool workWithMouseInput = false;
    ////


    void Start()
    {
        cassetteAmount = cassettes.Count;
        currentFocus = cassetteAmount - 1;
    }


    void Update()
    {
        TouchControls();


        // Temporary
        ////
        if (workWithMouseInput)
        MouseControls();
        ////
    }

    private void MouseControls()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            if (currentFocus >= -1 && currentFocus < cassetteAmount - 1)
            {
                currentFocus += 1;
                cassettes[currentFocus].transform.Rotate(Vector3.right, rotationAmount);
            }
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            if (currentFocus > 0 && currentFocus <= cassetteAmount - 1)
            {
                currentFocus -= 1;
                cassettes[currentFocus + 1].transform.Rotate(Vector3.right, -rotationAmount);
            }
        }
    }

    private void TouchControls()
    {
        if (Input.touchCount > 0)                       // Do we have input?
        {
            Touch myTouch = Input.touches[0];

            Vector3 touchPosition = myTouch.position;

            if (myTouch.phase == TouchPhase.Began)
            {
                origPosition = touchPosition;
            }
            if (myTouch.phase == TouchPhase.Moved)
            {
                touchPosition = myTouch.position;
                if ((touchPosition.y - origPosition.y) >= minimumMovementForChange)
                {
                    if (currentFocus >= -1 && currentFocus < cassetteAmount - 1)

                    {

                        currentFocus += 1;

                        cassettes[currentFocus].transform.Rotate(Vector3.right, rotationAmount);
                    }
                }
                if ((touchPosition.y - origPosition.y) < minimumMovementForChange)
                {
                    if (currentFocus > 0 && currentFocus <= cassetteAmount - 1)

                    {

                        currentFocus -= 1;

                        cassettes[currentFocus + 1].transform.Rotate(Vector3.right, -rotationAmount);
                    }
                }
            }
            if (myTouch.phase == TouchPhase.Ended)
                touchPosition = new Vector3(Vector3.zero.x, touchPosition.y, Vector3.zero.z);
        }
    }

}