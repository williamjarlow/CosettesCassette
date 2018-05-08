using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMovement : MonoBehaviour {

    [SerializeField] private float msDecider;
    [SerializeField] private float randomMS;
    [SerializeField] private float moveFrom;
    [SerializeField] private float moveTo;
    [HideInInspector] public float speed;
    [HideInInspector] public int points;

    // Use this for initialization
    void Start ()
    {

        msDecider = transform.position.x;
        randomMS = Random.Range(moveFrom, moveTo);

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
        }

    }

    void Left()
    {
        transform.Translate(Vector3.up * speed);

        transform.position = new Vector3(-Mathf.PingPong(Time.time, randomMS), transform.position.y, transform.position.z);

    }

    void Right()
    {
       transform.Translate(Vector3.up * speed);

        transform.position = new Vector3(Mathf.PingPong(Time.time, randomMS), transform.position.y, transform.position.z);

    }
}
