using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {
    [SerializeField] GameObject UITarget;
    
    [SerializeField] private bool isDual;
    [Header("Settings for DualButtons")]
    [SerializeField] public Material unselectedMaterial;
    [SerializeField] public Material selectedMaterial;
    [SerializeField] GameObject UITargetDual;
    private enum buttonstates { Up, Down, Moving }
    buttonstates buttonstate;
    bool startingPos;

    private Vector3 Startingposition;
    [SerializeField] float buttonPressdepth;
    private Vector3 endposition;
    [SerializeField] private int buttonSpeed;
    private bool buttonclicked = false;
    // Use this for initialization
    void Start () {
        buttonstates buttonstate;
        buttonstate = buttonstates.Up;
        Startingposition = transform.position;
        endposition = new Vector3(Startingposition.x, Startingposition.y, Startingposition.z + buttonPressdepth);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        switch (buttonstate)
        {
            //Move buttons up or down
            case buttonstates.Up:
                gameObject.GetComponent<MeshRenderer>().material = unselectedMaterial;
                break;
            case buttonstates.Moving:
                if (startingPos)
                {
                    transform.position = Vector3.MoveTowards(transform.position, endposition, buttonSpeed);
                    if (transform.position == endposition) buttonstate = buttonstates.Down;
                }
                else if (!startingPos)
                {
                    transform.position = Vector3.MoveTowards(transform.position, Startingposition, buttonSpeed);
                    if (transform.position == Startingposition) buttonstate = buttonstates.Up;
                }
                break;
            case buttonstates.Down:
                gameObject.GetComponent<MeshRenderer>().material = selectedMaterial;
                if (!isDual)
                {
                    
                    startingPos = false;
                    buttonstate = buttonstates.Moving;
                }
                else { }
                break;
        }
	}
    private void OnMouseOver()
    {
        if (buttonstate != buttonstates.Moving)
        {


            if (!isDual)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    UITarget.GetComponent<Button>().onClick.Invoke();
                    buttonstate = buttonstates.Moving;
                    startingPos = true;
                }
            }
            else if (isDual)
            {

                if (Input.GetMouseButtonDown(0))
                {
                    if (buttonclicked)
                    {
                        UITargetDual.GetComponent<Button>().onClick.Invoke();
                        //buttonclicked = false;
                        startingPos = false;
                        buttonclicked = false;
                        buttonstate = buttonstates.Moving;
                    }
                    else if (!buttonclicked)
                    {
                        UITarget.GetComponent<Button>().onClick.Invoke();
                        buttonclicked = true;
                        startingPos = true;
                        buttonstate = buttonstates.Moving;

                    }
                }

            }
        }

    }
    public void ToggleButtonUp()
    {
        
            UITargetDual.GetComponent<Button>().onClick.Invoke();
            //buttonclicked = false;
            startingPos = false;
            buttonclicked = false;
            buttonstate = buttonstates.Moving;
        
        
    }
}
