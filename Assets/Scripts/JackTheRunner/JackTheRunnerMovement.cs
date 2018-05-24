using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackTheRunnerMovement : MonoBehaviour {

	private AudioManager audioManager;

    private Rigidbody2D rb;
    private JackTheRunner mainGame;
    private Color changedColor;
    private Color origColor;
    private SpriteRenderer sprite;
    private Vector3 jump = new Vector3(0.0f, 1.0f, 0.0f);
    private LayerMask groundLayer;
    private bool grounded = true;
    private bool invisible = false;

    [SerializeField] private float jumpForce = 4.5f;
    [SerializeField] private float invisibilitySeconds = 1;
    [SerializeField] private float invisibilityBlinkSpeed = 5;
    [SerializeField] private GameObject jacksHurt;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        changedColor = GetComponent<SpriteRenderer>().color;
        origColor = GetComponent<SpriteRenderer>().color;
        sprite = GetComponent<SpriteRenderer>();
        mainGame = GetComponentInParent<JackTheRunner>();
		audioManager = mainGame.gameManager.audioManager;
        jacksHurt.SetActive(false);
	}

    void Update()
    {

        // For testing purposes only
        //
		if (Input.GetKeyDown ("space") && grounded)
		{
			rb.AddForce (jump * jumpForce, ForceMode2D.Impulse);
			audioManager.PlayRunnerJump();
			print ("jump");
		}
        //

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && grounded)
		{
			rb.AddForce (jump * jumpForce, ForceMode2D.Impulse);
			audioManager.PlayRunnerJump();
		}

        if (invisible)
        {
            changedColor.a = Mathf.Lerp(1, 0.2f, Mathf.PingPong(Time.time * invisibilityBlinkSpeed, 1));
            sprite.color = changedColor;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        grounded = true;
    }

	private void OnCollisionEnter2D (Collision2D collision)
	{
		audioManager.PlayRunnerLand();
		print ("land");
	}

    private void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!invisible)
        {
            mainGame.RegisterEvent(eventTriggered.jackDamaged);
            jacksHurt.SetActive(true);
            invisible = true;
            StartCoroutine(invisibilityTime(invisibilitySeconds));
			audioManager.PlayRunnerHurt ();
        }
    }

    IEnumerator invisibilityTime(float time)
    {
        yield return new WaitForSeconds(time);
        jacksHurt.SetActive(false);
        invisible = false;
        sprite.color = origColor;
    }


}
