using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCamera : MonoBehaviour {

    [SerializeField] private Camera zoomedOut;
    [SerializeField] private Camera closeUp;
    [SerializeField] private float delayCounter;
    [SerializeField] private float delayCounterWait;
    [SerializeField] private float backMovement;
    private Vector3 startPos;
    public bool zoomed = false;
    

    void Start()
    {
        zoomedOut.enabled = false;
        closeUp.enabled = true;
        startPos = closeUp.transform.position;
    }

    public void cameraSwap()
    {
        swap();
        if (zoomed == true)
        {
            StartCoroutine(MoveFromTo(new Vector3(zoomedOut.transform.localPosition.x, zoomedOut.transform.localPosition.y, backMovement), zoomedOut.transform.localPosition, delayCounter));
            StartCoroutine(Delay());
            //swapBack();
        }
        //swapBack();
    }
    
    public void swap()
    {
        zoomedOut.enabled = true;
        closeUp.enabled = false;
        zoomed = true;


    }

    public void swapBack()
    {
        zoomedOut.enabled = false;
        closeUp.enabled = true;
    }

    IEnumerator MoveFromTo(Vector3 pointA, Vector3 pointB, float time)
    {
        bool moving = false;
        if (!moving)
        {                     // Do nothing if already moving
            moving = true;                 // Set flag to true
            float t = 1.0f;
            while (t >= 0.0f)
            {
                t -= Time.deltaTime / time; // Sweeps from 0 to 1 in time seconds
                zoomedOut.transform.localPosition = Vector3.Lerp(pointA, pointB, t); // Set position proportional to t
                //cassettes[chosen].transform.localRotation = Quaternion.Slerp(fromRot, targetRot, t);
                yield return new WaitForEndOfFrame();
            }
            moving = false;
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delayCounterWait);
        StartCoroutine(MoveFromTo(startPos, new Vector3(zoomedOut.transform.localPosition.x, zoomedOut.transform.localPosition.y, backMovement), delayCounter));
        yield return new WaitForSeconds(2.5f);
        swapBack();
    }

}
