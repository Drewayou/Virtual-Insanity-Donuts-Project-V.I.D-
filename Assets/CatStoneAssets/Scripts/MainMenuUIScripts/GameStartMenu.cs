using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject start;
    public GameObject tutorial;
    public GameObject mainMenu;
    public GameObject settings;
    public GameObject about;
    public GameObject quit;

    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button settingsButton;
    public Button aboutButton;
    public Button quitButton;
    
    [SerializeField]
    [Tooltip("Drag and drop the button pressed UI here.")]
    public GameObject buttonSFX;

    public List<Button> returnButtons;

    // Start is called before the first frame update
    void Start()
    {
        EnableMainMenu();

        settingsButton.onClick.AddListener(EnableOption);
        aboutButton.onClick.AddListener(EnableAbout);
        quitButton.onClick.AddListener(QuitGame);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainRoomScene");
    }

    public void HideAll()
    {
        start.SetActive(false);
        tutorial.SetActive(false);
        mainMenu.SetActive(false);
        settings.SetActive(false);
        about.SetActive(false);
        quit.SetActive(false);
    }

    public void EnableMainMenu()
    {
        start.SetActive(true);
        tutorial.SetActive(true);
        mainMenu.SetActive(true);
        settings.SetActive(true);
        about.SetActive(true);
        quit.SetActive(true);
    }
    public void EnableOption()
    {
        HideAll();
        settings.SetActive(true);
    }
    public void EnableAbout()
    {
        HideAll();
        about.SetActive(true);
    }
    public void PlayButtonPressedSFX(){
        Instantiate(buttonSFX);
    }
}
