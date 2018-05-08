using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMovement : MonoBehaviour {


    [SerializeField] private float randomMS;
    [SerializeField] private float edge;
    private Vector3 bounce;
    private bool bouncy = true;
    [HideInInspector] public float speed;
    [HideInInspector] public float speedx = 0.1f;
    [HideInInspector] public int points;
	// Use this for initialization
	void Start ()
    {
        bounce = this.transform.localPosition;
        randomMS = Random.Range(2, 4);


    }

    // Update is called once per frame
    void Update ()
    {
        transform.Translate(Vector3.up * speed);

        if (bouncy == true )
        {
            transform.Translate(Vector3.left * speedx);

            if(this.transform.localPosition.x < bounce.x - randomMS || this.transform.localPosition.x < -edge)
            {
                bouncy = false;
            }
        }
        else if (bouncy == false)
        {
            transform.Translate(Vector3.right * speedx);

            if (this.transform.localPosition.x > bounce.x + randomMS || this.transform.localPosition.x > edge)
            {
                bouncy = true;
            }
        }

    }

}
