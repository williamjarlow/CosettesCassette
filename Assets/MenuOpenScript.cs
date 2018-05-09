using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOpenScript : MonoBehaviour {
    private bool Opening = false;
    private bool Closing = false;
    Animator animator;
    Vector3 startingPosition;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private float speed;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        startingPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (Opening == true)
        {
            transform.position.Set(endPosition.x, transform.position.y, transform.position.z);
        }
        else if(Closing == true)
        {
            Vector3 newPosition; newPosition = Vector3.MoveTowards(transform.position, startingPosition, speed);
            transform.position.Set(newPosition.x, newPosition.y, newPosition.z);
        }
	}
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Opening == false && Closing == false)
            {
                animator.SetBool("OpenTrigger", true);
                Opening = true;
            }
            else if (Opening == true && Closing == false)
            {
                animator.SetBool("CloseTrigger", true);
                animator.SetBool("OpenTrigger", false);
                Closing = true;
                Opening = false;
            }
            else if (Closing == true && Opening == false)
            {
                
                animator.SetBool("OpenTrigger", true);
                animator.SetBool("CloseTrigger", false);
                Opening = true;
                Closing = false;
            }
        }
    }


}
