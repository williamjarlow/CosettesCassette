using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineRendererTest : MonoBehaviour {
    float rotation = 0;
    float scale = 10;
    Vector3 offset = new Vector3(0.5f, 0.5f, 0);
    Vector3 tiling = new Vector3(1, 1, 1);
    Material animMat;
    LineRenderer lr;

    // Use this for initialization
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        animMat = lr.material;
        //Debug.Log ("Material used: "+animMat.name);
    }

    void Update()
    {
        rotation += Time.deltaTime * scale;


        Quaternion quat = Quaternion.Euler(0, 0, rotation);
        Matrix4x4 matrix1 = Matrix4x4.TRS(offset, Quaternion.identity, tiling);
        Matrix4x4 matrix2 = Matrix4x4.TRS(Vector3.zero, quat, tiling);
        Matrix4x4 matrix3 = Matrix4x4.TRS(-offset, Quaternion.identity, tiling);
        animMat.SetMatrix("UNITY_MATRIX_MVP", matrix1 * matrix2 * matrix3);
    }
}
