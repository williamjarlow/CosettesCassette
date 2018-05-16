using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    [SerializeField] private Image black;
    [SerializeField] private Animator anim;
    //[SerializeField] private int index;

    public void LoadByIndex(int index)
    {
        Time.timeScale = 1.0F;
        StartCoroutine(Fading(index));
    }

    IEnumerator Fading(int index)
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        SceneManager.LoadScene(index);

    }
}
