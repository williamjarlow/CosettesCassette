using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    public void LoadByIndex(int sceneToLoad)
    {
        Time.timeScale = 1.0F;
        SceneManager.LoadScene(sceneToLoad);
    }
}
