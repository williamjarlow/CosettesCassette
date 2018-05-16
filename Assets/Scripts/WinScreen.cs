using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour {

    // Use this for initialization
    void Start ()
    {
        
        
    }

    // Update is called once per frame
    void Update ()
    {

    }

    public void EnableWinScreen ()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        GetComponent<Text>().enabled = true;
    }

    public void DisableWinScreen()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        GetComponent<Text>().enabled = false;
    }

}
