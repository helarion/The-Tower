using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] bool hasPlayedTutorialAlready = false;
    [SerializeField] GameObject popupPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider MusicSlider;

    [Header("Buttons")]
    [SerializeField] Button StartButton;
    [SerializeField] Button TutoButton;
    [SerializeField] Button OptionsButton;
    [SerializeField] Button CloseOptionsButton;
    [SerializeField] Button YesButton;
    [SerializeField] Button NoButton;
    [SerializeField] Button FastButton;
    [SerializeField] Button FancyButton;

    [SerializeField] AudioSource source;

    private void Start()
    {
        //Sliders
        SFXSlider.onValueChanged.AddListener(UpdateSoundVolume);
        MusicSlider.onValueChanged.AddListener(UpdateMusicVolume);

        //Buttons
        StartButton.onClick.AddListener(StartGameButton);
        OptionsButton.onClick.AddListener(OptionScreen);
        CloseOptionsButton.onClick.AddListener(BackMenu);
        YesButton.onClick.AddListener(LaunchGame);
        NoButton.onClick.AddListener(LaunchTutorial);
        FastButton.onClick.AddListener(FastGraphics);
        FancyButton.onClick.AddListener(FancyGraphics);
    }

    void ButtonClick()
    {
        source.Play();
    }

    public void StartGameButton()
    {
        ButtonClick();
        if(hasPlayedTutorialAlready)
        {
            LaunchGame();
        }
        else
        {
            popupPanel.SetActive(true);
        }
    }

    public void LaunchGame()
    {
        ButtonClick();
        SceneManager.LoadScene(1);
    }

    public void LaunchTutorial()
    {
        ButtonClick();
        SceneManager.LoadScene(2);
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
