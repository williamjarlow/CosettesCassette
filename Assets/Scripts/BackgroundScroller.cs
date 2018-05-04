using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour {


    [TextArea]
    public string Notes = "Apply to a 3D Quad, stretch Quad to desired resolution. Add background to Mesh Renderer and go!";

    [SerializeField] private float scrollSpeed = 0.05f;
    public Vector2 direction = new Vector2(1, 1);
    private Vector2 offset;
    private Renderer texture;

    private void Start()
    {
        texture = GetComponent<Renderer>();
    }

    void Update ()
    {
        offset = new Vector2(direction.x * Time.time * scrollSpeed, direction.y * Time.time * scrollSpeed);
        texture.material.mainTextureOffset = offset;
	}
}
