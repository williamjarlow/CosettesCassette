using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectLoadScene : MonoBehaviour {

    // Used to load the correct scene when a raycast hit this object
    public int LoadSceneIndex;

    // Used to determine if this gameobject is currently focused 
    public bool isFocused;

    private void Start()
    {
        isFocused = false;
    }
}
