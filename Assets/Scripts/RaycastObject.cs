using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaycastObject : MonoBehaviour
{
    [SerializeField] private GameObject LevelSelectMenu;
    [SerializeField] private int loadLevelIndex;
    private LevelSelect LevelSelectScript;

    void Start()
    {

        LevelSelectScript = LevelSelectMenu.GetComponent<LevelSelect>();
    }

    void Update()
    {
 
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

    void SwitchScene()
    {

        SceneManager.LoadScene(loadLevelIndex);
    }
}
