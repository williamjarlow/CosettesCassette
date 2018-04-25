﻿using System.Collections;
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
        credit = credits.GetComponent<Credits>();
        now = credit.currentPos;
        


        if (now > 4000)
        {
            Winning();
        }
        else if (now < 4000)
        {
            GetComponent<Text>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }

    }

    void Winning ()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        GetComponent<Text>().enabled = true;
    }
}