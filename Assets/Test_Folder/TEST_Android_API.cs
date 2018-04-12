using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TEST_Android_API : MonoBehaviour {

    private static AndroidJavaObject currentActivity
    {
        get
        {
            return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        }
    }

    void Start ()
    {
        //Debug.Log(GetProperty("PROPERTY_OUTPUT_FRAMES_PER_BUFFER"));
        Debug.Log(GetPackageName());
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    // Keys: https://developer.android.com/reference/android/media/AudioManager.html#getProperty(java.lang.String)
    public static string GetProperty(string key)
    {
        return currentActivity.Call<string>("getProperty", key, 0);
    }

    public static string GetPackageName()
    {
        return currentActivity.Call<string>("getPackageName");
    }
}
