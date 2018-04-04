using UnityEngine;
using System.Collections;

public class MenuAppearScript : MonoBehaviour
{

    [SerializeField] private GameObject menuToShow;
    [SerializeField] private GameObject menuToHide;
    [SerializeField] private string button;
    private bool isShowing = true;

    void Start()
    {
        menuToShow.SetActive(true);
        menuToHide.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(button))
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
}