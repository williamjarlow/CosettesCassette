using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMovement : MonoBehaviour {


    private float randomMS;
    [SerializeField] private float leftEdge;
    [SerializeField] private float rightEdge;
    [SerializeField] private float lowerBound;
    [SerializeField] private float upperBound;
    [SerializeField] private float speedx;
    float spriteWidth;
    private bool bouncy = true;
    [HideInInspector] public float speed;
    [HideInInspector] public int points;
	// Use this for initialization
	void Start ()
    {
        spriteWidth = GetComponent<SpriteRenderer>().sprite.bounds.extents.x;
        randomMS = Random.Range(lowerBound, upperBound);
        int i = Random.Range(0, 2);
        if (i == 0)
            bouncy = false;

    }

    // Update is called once per frame
    void Update ()
    {
        transform.Translate(Vector3.up * speed);

        if (bouncy == true )
        {
            transform.Translate(Vector3.left * speedx);
            if(0 < -randomMS || transform.position.x < leftEdge + spriteWidth)
            {
                bouncy = false;
            }
        }
        else if (bouncy == false)
        {
            transform.Translate(Vector3.right * speedx);

            if (0 > randomMS || transform.position.x > rightEdge - spriteWidth)
            {
                bouncy = true;
            }
        }

    }

}
