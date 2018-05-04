using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMovement : MonoBehaviour {

    [SerializeField] private float msDecider;
    [SerializeField] private float randomMS;
    [SerializeField] private float speed;
    [HideInInspector] public int points;
	// Use this for initialization
	void Start ()
    {

        msDecider = transform.position.x;
        randomMS = Random.Range(0, 7);

    }
	
	// Update is called once per frame
	void Update ()
    {

        if (msDecider < 0)
        {
            Left();
        }
        else if (msDecider > 0)
        {
            Right();
        }
        else
        {
            this.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.up * speed);
        }

    }

    void Left()
    {
        //transform.position = new Vector3(-Mathf.PingPong(Time.time, randomMS), transform.position.y, transform.position.z);

    }

    void Right()
    {
        transform.position = new Vector3(Mathf.PingPong(Time.time, randomMS), transform.position.y, transform.position.z);

    }
}
