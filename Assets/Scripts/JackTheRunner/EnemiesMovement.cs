using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesMovement : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Collider2D coll;
    private Rigidbody2D rb;
    [HideInInspector] public float movementSpeed = -1.5f;
    [SerializeField] private Sprite deadSprite;

	void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == ("JackTheRunner") && deadSprite != null)
        {
            animator.enabled = false;
            coll.enabled = false;
            spriteRenderer.sprite = deadSprite;
            rb.AddTorque(Random.Range(-100, 300), ForceMode2D.Force);
        }

    }


        void Update ()
    {
        transform.Translate(movementSpeed * Time.deltaTime, 0, 0);
	}
}
