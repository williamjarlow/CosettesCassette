﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawn : MonoBehaviour {


    private Vector3 gravity;
    [SerializeField] private GameObject ducks;
    [SerializeField] private int duckAmount;
    [SerializeField] private float speed;
    [SerializeField] private float randomMinX;
    [SerializeField] private float randomMaxX;
    [SerializeField] private float spawnTime;


    // Use this for initialization
    void Start ()
    {
        gravity.y = transform.position.y;
        SpawnNotes();
        StartCoroutine(DuckWait());
    }

    // Update is called once per frame
    void Update ()
    {
        
    }

    public void SpawnNotes()
    {
            gravity.x = Random.Range(randomMinX, randomMaxX);
            ducks = Instantiate(ducks, new Vector3(gravity.x,gravity.y,0), Quaternion.identity);
            ducks.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.up * speed);    
    }

    IEnumerator DuckWait()
    {
        for (int i = 0; i < duckAmount; i++)
        {
            yield return new WaitForSeconds(spawnTime);
            SpawnNotes();
        }
    }

}