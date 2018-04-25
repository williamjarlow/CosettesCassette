using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons_Movement : MonoBehaviour {
    public bool isUp = true;
    [SerializeField] float ButtonSpeed;
    
    Vector3 button_Startposition;
    Vector3 local_target;
    [SerializeField] Vector3 button_Endposition;

	// Use this for initialization
	void Start () {
        button_Startposition = transform.position;
        local_target = button_Endposition - transform.position;

    }
	
	// Update is called once per frame
	void Update () {
        if (isUp)
        {
            if(transform.position != button_Startposition)
            {
                transform.position = Vector3.MoveTowards(transform.position, button_Startposition, ButtonSpeed);
            }
            
        }
        else if (!isUp)
        {
            if (transform.position != button_Endposition)
            {
                transform.position = Vector3.MoveTowards(transform.position, button_Endposition, ButtonSpeed);
            }
        }
	}

    public void setButtonEnabled()
    {
        if (isUp) isUp = false;
        else if (!isUp) isUp = true;

    }
}
