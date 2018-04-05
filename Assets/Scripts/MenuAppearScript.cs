using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuAppearScript : MonoBehaviour
{

    [SerializeField] private GameObject menuToHide;
    [SerializeField] private GameObject menuToShow;
    [SerializeField] private string keyboardButton;
    [SerializeField] private Button UIbutton;
    private bool isShowing = true;

    void Start()
    {
        menuToShow.SetActive(true);
        menuToHide.SetActive(false);
        Button btn = UIbutton.GetComponent<Button>();
        btn.onClick.AddListener(() => Pause());
    }

    void Update()
    {
        if (Input.GetKeyDown(keyboardButton))
        {
            if (Time.timeScale == 0.0F)
            {
                Time.timeScale = 1.0F;
            }
            else
            {
                Time.timeScale = 0.0F;
            }
            isShowing = !isShowing;
            menuToShow.SetActive(isShowing);
            menuToHide.SetActive(!isShowing);
        }
    }

    void Pause()
    {
        if (Time.timeScale == 0.0F)
        {
            Time.timeScale = 1.0F;
        }
        else
        {
            Time.timeScale = 0.0F;
        }
        isShowing = !isShowing;
        menuToShow.SetActive(isShowing);
        menuToHide.SetActive(!isShowing);
    }
}