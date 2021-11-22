using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] GameObject popupPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider MusicSlider;

    [Header("Buttons")]
    [SerializeField] Button StartButton;
    [SerializeField] Button OptionsButton;
    [SerializeField] Button CloseOptionsButton;
    [SerializeField] Button SkipTutorialButton;
    [SerializeField] Button PlayTutorialButton;
    [SerializeField] Button FastButton;
    [SerializeField] Button FancyButton;
    [SerializeField] Button QuitButton;
    [SerializeField] Button ENButton;
    [SerializeField] Button FRButton;

    [SerializeField] AudioSource source;

    [SerializeField]

    private void Awake()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }

    private void Start()
    {
        buttonHighlight();

        //Sliders
        SFXSlider.onValueChanged.AddListener(UpdateSoundVolume);
        MusicSlider.onValueChanged.AddListener(UpdateMusicVolume);

        //Buttons
        StartButton.onClick.AddListener(StartGameButton);
        OptionsButton.onClick.AddListener(OptionScreen);
        CloseOptionsButton.onClick.AddListener(BackMenu);
        SkipTutorialButton.onClick.AddListener(SkipTutorial);
        PlayTutorialButton.onClick.AddListener(LaunchTutorial);
        FastButton.onClick.AddListener(FastGraphics);
        FancyButton.onClick.AddListener(FancyGraphics);
        QuitButton.onClick.AddListener(Application.Quit);
        ENButton.onClick.AddListener(SwitchLanguageToEnglish);
        FRButton.onClick.AddListener(SwitchLanguageToFrench);

    }

    void buttonHighlight()
    {
        Image iFR = FRButton.GetComponent<Image>();
        Image iEN = ENButton.GetComponent<Image>();
        Color test1;
        Color test2;
        if (MusicHandler.Instance.language == 0)
        {
            test1 = iFR.color;
            test1.a = 0.3f;
            iFR.color = test1;
            test2 = iEN.color;
            test2.a = 1f;
            iEN.color = test2;
        }
        else if (MusicHandler.Instance.language == 1)
        {
            test1 = iEN.color;
            test1.a = 0.3f;
            iEN.color = test1;
            test2 = iFR.color;
            test2.a = 1f;
            iFR.color = test2;
        }
    }

    void SwitchLanguageToEnglish()
    {
        ButtonClick();
        MusicHandler.Instance.language = 0;
        PlayerPrefs.SetInt("Language", 0);
        buttonHighlight();
    }

    void SwitchLanguageToFrench()
    {
        ButtonClick();
        MusicHandler.Instance.language = 1;
        PlayerPrefs.SetInt("Language", 1);
        buttonHighlight();
    }

    void ButtonClick()
    {
        source.Play();
    }

    public void StartGameButton()
    {
        ButtonClick();
        if(!MusicHandler.Instance.playTutorial)
        {
            LaunchGame();
        }
        else
        {
            popupPanel.SetActive(true);
        }
    }

    public void LaunchTutorial()
    {
        PlayerPrefs.SetInt("PlayedTutorial", 0);
        MusicHandler.Instance.playTutorial = true;
        LaunchGame();
    }

    public void SkipTutorial()
    {
        PlayerPrefs.SetInt("PlayedTutorial", 1);
        MusicHandler.Instance.playTutorial = false;
        LaunchGame();
    }

    public void LaunchGame()
    {
        ButtonClick();
        SceneManager.LoadScene(1);
    }

    public void OptionScreen()
    {
        ButtonClick();
        menuPanel.SetActive(true);
    }

    public void BackMenu()
    {
        ButtonClick();
        menuPanel.SetActive(false);
    }

    public void FastGraphics()
    {
        ButtonClick();
        QualitySettings.SetQualityLevel(0, true);
    }

    public void FancyGraphics()
    {
        ButtonClick();
        QualitySettings.SetQualityLevel(5, true);
    }

    void UpdateMusicVolume(float value)
    {
        MusicHandler.Instance.UpdateMusicVolume(value);
    }

    void UpdateSoundVolume(float value)
    {
        MusicHandler.Instance.UpdateSFXVolume(value);
    }
}
