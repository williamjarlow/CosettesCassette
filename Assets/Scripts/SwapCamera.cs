using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCamera : MonoBehaviour {

    [SerializeField] private GameObject visual;
    [SerializeField] private GameObject FakeCanvas;
    [SerializeField] private Camera zoomedOut;
    [SerializeField] private Camera closeUp;
    [SerializeField] private float delayCounter;
    [SerializeField] private float delayCounterWait;
    [SerializeField] private float delayCounterWaitatStart;
    [SerializeField] private float backMovement;
    [SerializeField] private Vector3 origPos;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private bool zoomed = false;
    

    void Start()
    {
        zoomedOut.enabled = true;
        visual.SetActive(false);
        closeUp.enabled = false;
        zoomedOut.transform.localPosition = origPos;
        StartCoroutine(DelayatStart());
        startPos = closeUp.transform.position;
        Debug.Assert(visual != null,"Add the visual effects to swap camera");
    }

    public void cameraSwap()
    {
        FakeCanvas.SetActive(true);
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
        visual.SetActive(false);
        closeUp.enabled = false;
        zoomed = true;


    }

    public void swapBack()
    {
        zoomedOut.enabled = false;
        visual.SetActive(true);
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
        FakeCanvas.SetActive(false);
    }

    IEnumerator DelayatStart()
    {
        yield return new WaitForSeconds(delayCounterWaitatStart);
        StartCoroutine(MoveFromTo(closeUp.transform.localPosition, origPos, delayCounter));
        yield return new WaitForSeconds(2.5f);
        zoomedOut.enabled = false;
        visual.SetActive(true);
        closeUp.enabled = true;
        FakeCanvas.SetActive(false);
    }

}
