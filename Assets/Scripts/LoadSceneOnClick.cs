using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
	private AudioManager audioManager;

    [SerializeField] private Image black;
    [SerializeField] private Animator anim;
    //[SerializeField] private int index;

	private void Start()
	{
		audioManager = GameObject.FindWithTag ("AudioManager").GetComponent<AudioManager> ();
	}

    public void LoadByIndex(int index)
    {
        Time.timeScale = 1.0F;
		audioManager.UnloadBanks ();
        StartCoroutine(Fading(index));
    }

    public void ReloadScene()
    {
        // Can't load same scene without this for some reason... Needed for level select
        audioManager.UnloadBanks();
        StartCoroutine(Fading(1));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator Fading(int index)
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        SceneManager.LoadScene(index);
    }
}
