using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Duration
{
    public float start;
    public float stop;
}

[System.Serializable]
public class Corruption
{
    public GameObject corruptionType;
    public Duration duration;
}

public class CorruptionHandler : MonoBehaviour {
    [SerializeField]
    private List<Corruption> corruptions;
    void Start()
    {
        foreach(Corruption corruption in corruptions)
        {
            GameObject go = Instantiate (corruption.corruptionType,  gameObject.transform); //instantiate all of the corruptions
            go.GetComponent<CorruptionBaseClass>().duration = corruption.duration; //set initial values of all the corruptions
        }
    }
}
