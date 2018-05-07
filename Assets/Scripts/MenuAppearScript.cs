using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuAppearScript : MonoBehaviour
{

    [SerializeField] private GameObject menuToHide;
    [SerializeField] private GameObject menuToShow;
    [SerializeField] private string keyboardButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button unPauseButton;
    [SerializeField] private Text UIbuttonText;
    private bool isShowing = false;

    [SerializeField] private GameObject cassetteAnimation;
    void Start()
    {
        //menuToShow.SetActive(true);
        //menuToHide.SetActive(false);
      //  Button btn = pauseButton.GetComponent<Button>();
      //  btn.onClick.AddListener(() => Pause());
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(keyboardButton))
        {
            if (Time.timeScale == 0.0F)
            {
                Time.timeScale = 1.0F;
            }
            
            {
                Time.timeScale = 0.0F;
            }
            isShowing = !isShowing;
            menuToShow.SetActive(isShowing);
            menuToHide.SetActive(!isShowing);
        }
        */

    }


    /*
    void Pause()
    {
        Text btnText = UIbuttonText.GetComponent<Text>();

        if (Time.timeScale == 0.0F)
        {
            Time.timeScale = 1.0F;
        }
        else
        {
            Time.timeScale = 0.0F;
        }

        
        if (isShowing == true)
        {
            btnText.text = "Unpause";
        }
        else
        {
            btnText.text = "Pause";
        }
        

        isShowing = !isShowing;
        menuToShow.SetActive(isShowing);
        menuToHide.SetActive(!isShowing);
    }
    */

    public void TogglePauseScreen()
    {
        if (!isShowing)
        {
            ShowPauseScreen();
            //GameManager.Instance.audioManager.AudioUnpauseMusic();
        }

        else if (isShowing)
        {
            HidePauseScreen();
            //GameManager.Instance.audioManager.AudioPauseMusic();
        }
    }

    public void ShowPauseScreen()
    {
        isShowing = true;
        menuToShow.SetActive(true);
    }

    public void HidePauseScreen()
    {
        isShowing = false;
        menuToHide.SetActive(false);
    }

    public void StartCassette()
    {
        
        CassetteAnimation cassetteAnim = cassetteAnimation.GetComponent<CassetteAnimation>();
        cassetteAnim.cassetteAnimation = true;
    }

    public void StopCassette()
    {
        
        CassetteAnimation cassetteAnim = cassetteAnimation.GetComponent<CassetteAnimation>();
        cassetteAnim.cassetteAnimation = false;
    }
}