using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillScreen : MonoBehaviour {

    // Use this for initialization
    void Start ()
    {
        
        
    }

    // Update is called once per frame
    void Update ()
    {

    }

    public void Winning ()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        GetComponent<Text>().enabled = true;
    }

    public void ShutdownWinning()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        GetComponent<Text>().enabled = false;
    }

}
