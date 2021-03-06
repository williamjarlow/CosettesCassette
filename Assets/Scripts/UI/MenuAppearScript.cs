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
    [HideInInspector] public bool isShowing = false;

	private AudioManager audioManager;

    [SerializeField] private GameObject cassetteAnimation;
    void Start()
    {
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();

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
            audioManager.pausedMusic = true;
			audioManager.gameMusicEv.setPaused(true);
        }

        else if (isShowing)
        {
            HidePauseScreen();
            audioManager.pausedMusic = false;
			audioManager.gameMusicEv.setPaused(false);
        }
    }

    public void ShowPauseScreen()
    {
        isShowing = true;
        menuToShow.SetActive(true);

		audioManager.PlayPauseMenuOpen();
    }

    public void HidePauseScreen()
    {
        isShowing = false;
        menuToHide.SetActive(false);

		audioManager.PlayPauseMenuClose();
    }

	public void ToggleSkinMenu()
	{
		if (!isShowing)
		{
			ShowSkinMenu();
		}

		else if (isShowing)
		{
			HideSkinMenu();
		}
	}

	public void ShowSkinMenu()
	{
		isShowing = true;
		menuToShow.SetActive(true);

		audioManager.PlaySkinMenuOpen();
	}

	public void HideSkinMenu()
	{
		isShowing = false;
		menuToHide.SetActive(false);

		audioManager.PlaySkinMenuClose();
	}

    //public void StartCassette()
    //{
        
    //    CassetteAnimation cassetteAnim = cassetteAnimation.GetComponent<CassetteAnimation>();
    //    cassetteAnim.cassetteAnimation = true;
    //}

    //public void StopCassette()
    //{
        
    //    CassetteAnimation cassetteAnim = cassetteAnimation.GetComponent<CassetteAnimation>();
    //    cassetteAnim.cassetteAnimation = false;
    //}
}