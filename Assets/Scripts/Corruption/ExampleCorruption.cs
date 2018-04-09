using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleCorruption : CorruptionBaseClass {
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

    public override void  StartCorruption()
    {

    }

    public override void EndCorruption()
    {
        if (CheckCorruptionCleared())
        {
            corruptionCleared = true;
        }
    }

    bool CheckCorruptionCleared()
    {
        //Clear conditions go here
        return true;
    }

}
