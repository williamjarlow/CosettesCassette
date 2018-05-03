using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingStuff : MonoBehaviour {


    public Vector3 gravity;
    //public Rigidbody rb;
    [SerializeField] private GameObject ducks;
    [SerializeField] private int duckAmount;
    //[SerializeField] private GameObject canvas;
    //[SerializeField] private GameObject spawn;
    [SerializeField] private float speed;

    // Use this for initialization
    void Start ()
    {
        

        //rb = GetComponent<Rigidbody>();
        //rb.AddForce(gravity, ForceMode.Impulse);
        Fill();

    }

    // Update is called once per frame
    void Update ()
    {
        
        
    }

    public void Fill()
    {
        //duckies = new GameObject[ducks.Length];
        for (int i = 0; i < duckAmount; i++)
        {
            gravity.x = Random.Range(-5, 5);
            gravity.y = Random.Range(-5, 5);
            ducks = Instantiate(ducks, new Vector3(gravity.x,gravity.y,0), Quaternion.identity);//spawn.GetComponent<Transform>().transform);//canvas.GetComponent<Canvas>().transform);
            ducks.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.up * speed);    
            //duckies[i].transform.parent = GameObject.Find("Canvas").transform;
        }
    }

}
