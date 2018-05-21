using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesMovement : MonoBehaviour {

    private Transform trans;
    [SerializeField] private float movementSpeed = -1.5f;

	void Start ()
    {
        trans = transform;
	}
	

	void Update ()
    {


        trans.Translate(movementSpeed * Time.deltaTime, 0, 0);
	}
}
