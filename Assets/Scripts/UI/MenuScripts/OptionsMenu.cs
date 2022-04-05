using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("User Settings Asset")]
    public OptionSettings userSettings;

    [HideInInspector] public bool GamePaused = false;

    [Header("UI Elements")]
    public GameObject pauseMenuUI;
    public GameObject tutorialUI;
    public GameObject tutorialButton;
    public TMP_Text backButtonText;

    [Header("Sliders")]
    public Slider sensSlider;
    public Slider musicSlider;
    public Slider soundSlider;

    private InputManager.InputMode _lastInputMode = InputManager.InputMode.Player;

    private void Awake()
    {
        //Sync with user settings
        //if (sensSlider)
        //{
        //    sensSlider.value = userSettings.CameraSensitivity;
        //}
        //if (musicSlider)
        //{
        //    musicSlider.value = userSettings.MusicVolume;
        //}
        //if (soundSlider)
        //{
        //    soundSlider.value = userSettings.SoundVolume;
        //}

        if (SceneManager.GetActiveScene().buildIndex == Scenes.GameScene)
        {
            backButtonText.text = "QUIT";
        }
    }

    private void Start()
    {
        //Sync using PlayerPref
        sensSlider.value = PlayerPrefs.GetFloat("PlayerSensitivity", 10f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        OnMusicSliderChanged(musicSlider.value);
        OnSoundSliderChanged(soundSlider.value);
        OnCameraSensitivitySliderChanged(sensSlider.value);

        tutorialButton.SetActive(SceneManager.GetActiveScene().buildIndex == Scenes.GameScene);
    }

    #region Slider Callbacks

    public void OnMusicSliderChanged(float value)
    { //userSettings.MusicVolume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        userSettings.MusicAudioMixer.SetFloat("volume", 20f * Mathf.Log10(value));
        PlayerPrefs.Save();
    }

    public void OnSoundSliderChanged(float value)
    { //userSettings.MusicVolume = value;
        PlayerPrefs.SetFloat("SoundVolume", value);
        userSettings.SoundAudioMixer.SetFloat("volume", 20f * Mathf.Log10(value));
        PlayerPrefs.Save();
    }

    public void OnCameraSensitivitySliderChanged(float value)
    { //userSettings.MusicVolume = value;
        PlayerPrefs.SetFloat("PlayerSensitivity", value);
        Player.sensX = Player.sensY = value;
        PlayerPrefs.Save();
    }

    #endregion Slider Callbacks

    public void OnFullScreenToggled(bool value)
    { }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void Update()
    {
        // Only do this in the game scene
        if (SceneManager.GetActiveScene().buildIndex != Scenes.GameScene) return;

        // Checks if the game is paused or not
        // In minigames we leave the minigame dont go to the menu
        if (Input.GetKeyDown(KeyCode.Escape) && InputManager.CurrentInputMode != InputManager.InputMode.Minigame)
        {
            if (GamePaused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);

        if (SceneManager.GetActiveScene().buildIndex == Scenes.GameScene)
        {
            SceneManager.LoadScene(Scenes.Lobby);
        }
    }

    public void Unpause()
    {
        pauseMenuUI.SetActive(false);

        if (SceneManager.GetActiveScene().buildIndex != Scenes.GameScene) return;

        if (tutorialUI != null) tutorialUI.SetActive(false);
        InputManager.CurrentInputMode = _lastInputMode;
        GamePaused = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);

        if (SceneManager.GetActiveScene().buildIndex != Scenes.GameScene) return;

        _lastInputMode = InputManager.CurrentInputMode;
        InputManager.CurrentInputMode = InputManager.InputMode.UI;
        GamePaused = true;
    }

    public void LoadMenu()
    {
    }

    public void ReturnToMainMenu()
    {
        CustomNetworkManager.Instance.Stop();
        SceneManager.LoadScene(Scenes.MainMenu);
    }

    public void Tutorial()
    {
        pauseMenuUI.SetActive(false);
        tutorialUI.SetActive(true);
    }

    public void QuitTutorial()
    {
        tutorialUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
}