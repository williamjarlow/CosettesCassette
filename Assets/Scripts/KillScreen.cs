using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillScreen : MonoBehaviour {

    [SerializeField] private Credits credits;
    private Credits credit;
    
    private float now;

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
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        GetComponent<Text>().enabled = true;
    }

    public void ShutdownWinning()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        GetComponent<Text>().enabled = false;
    }

}
