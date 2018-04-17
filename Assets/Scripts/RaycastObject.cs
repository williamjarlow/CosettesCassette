using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastObject : MonoBehaviour
{
    [SerializeField] private GameObject LevelSelectMenu;
    private LevelSelect LevelSelectScript;

    void Start()
    {

        LevelSelectScript = LevelSelectMenu.GetComponent<LevelSelect>();
    }

    void Update()
    {
        if (this.tag == "Cassette")
        {
            Rigidbody rigb;
            rigb = GetComponent<Rigidbody>();
            rigb.AddForce(transform.forward * 3);
            this.transform.Translate(Input.acceleration.x, 0, -Input.acceleration.z);
        }
    }

	void PlaceCassette()
    {
        Rigidbody rb;
        rb = GetComponent<Rigidbody>();

        LevelSelectScript.cassettes.Add(this);
        this.transform.parent = LevelSelectMenu.transform;
        this.transform.position = LevelSelectScript.cassettes[0].transform.position;
        Vector3 temp = this.transform.localPosition;
        temp.z = LevelSelectScript.cassettes[LevelSelectScript.cassettes.Count - 1].transform.localPosition.z - LevelSelectScript.cassettes.Count + 1;
        this.transform.localPosition = temp;
        this.transform.localRotation = Quaternion.AngleAxis(0, Vector3.right);
        this.transform.tag = ("PlacedCassette");
        rb.isKinematic = true;
    }
}
