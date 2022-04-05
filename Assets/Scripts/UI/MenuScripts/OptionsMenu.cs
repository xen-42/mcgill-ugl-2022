using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    [Header("User Settings Asset")]
    public OptionSettings userSettings;

    [HideInInspector] public bool GamePaused = false;

    [Header("UI Elements")]
    public GameObject pauseMenuUI;
    public GameObject tutorialUI;
    public GameObject tutorialButton;

    [Header("Sliders")]
    public Slider sensSlider;
    public Slider musicSlider;
    public Slider soundSlider;
    public Toggle fullscreenToggle;

    private InputManager.InputMode _lastInputMode = InputManager.InputMode.Player;

    private void Awake()
    {
        //Sync with user settings
        if (sensSlider)
        {
            sensSlider.value = userSettings.CameraSensitivity;
        }
        if (musicSlider)
        {
            musicSlider.value = userSettings.MusicVolume;
        }
        if (soundSlider)
        {
            soundSlider.value = userSettings.SoundVolume;
        }
        if(fullscreenToggle)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
        }
    }

    private void Start()
    {
        tutorialButton.SetActive(SceneManager.GetActiveScene().buildIndex == Scenes.GameScene);
    }

    #region Slider Callbacks

    public void OnMusicSliderChanged(float value) => userSettings.MusicVolume = value;

    public void OnSoundSliderChanged(float value) => userSettings.SoundVolume = value;

    public void OnCameraSensitivitySliderChanged(float value) => userSettings.CameraSensitivity = value;

    #endregion Slider Callbacks

    public void OnFullScreenToggled(bool value)
    { }

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
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
                Resume();
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