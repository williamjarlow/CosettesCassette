using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_Plugin : MonoBehaviour {

	void Start ()
    {
        TextMesh textMesh = GetComponent<TextMesh>();
        var plugin = new AndroidJavaClass("com.coscas.a16hangu.unityplugin.PluginClass");
        textMesh.text = plugin.CallStatic<string>("GetTextFromPlugin", 5);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
