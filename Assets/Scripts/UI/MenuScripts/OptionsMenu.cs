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

    //public AudioMixer musicAudioMixer;
    //public AudioMixer soundAudioMixer;
    public Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;
    //public GameObject playercharacter;

    [Header("Sliders")]
    public Slider sensSlider;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundSlider;

    private InputManager.InputMode _lastInputMode = InputManager.InputMode.Player;

    private void Awake()
    {
        //Sync with user settings
        if (sensSlider)
        {
            //userSettings.CameraSensitivity = userSettings.CameraSensitivity;
            sensSlider.value = userSettings.CameraSensitivity;
        }
        if (masterSlider)
            masterSlider.value = userSettings.MasterVolume;
        if (musicSlider)
            musicSlider.value = userSettings.MusicVolume;
        if (soundSlider)
            soundSlider.value = userSettings.SoundVolume;
    }

    private void Start()
    {  //Sync with user settings
        //if (sensSlider)
        //{
        //    userSettings.CameraSensitivity = userSettings.CameraSensitivity;
        //    sensSlider.value = userSettings.CameraSensitivity;
        //}
        //if (masterSlider)
        //    masterSlider.value = userSettings.MasterVolume;
        //if (musicSlider)
        //    musicSlider.value = userSettings.MusicVolume;
        //if (soundSlider)
        //    soundSlider.value = userSettings.SoundVolume;
        // QualitySettings.SetQualityLevel(3);

        //Get array of resolutions
        if (resolutionDropdown != null)
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            //if (sensSlider != null)
            //{
            //    sensSlider.value = (float)(Player.Instance.sensX);
            //}
        }

        //pauseMenuUI.SetActive(false);
        //tutorialUI.SetActive(false);
    }

    #region Slider Callbacks

    public void OnMasterSliderChanged(float value) => userSettings.MasterVolume = value;

    public void OnMusicSliderChanged(float value) => userSettings.MusicVolume = value;

    public void OnSoundSliderChanged(float value) => userSettings.SoundVolume = value;

    public void OnCameraSensitivitySliderChanged(float value) => userSettings.CameraSensitivity = value;

    #endregion Slider Callbacks

    public void OnQualityChanged(int value)
    { }

    public void OnResolutionChanged(int value)
    { }

    public void OnFullScreenToggled(bool value)
    { }

    public void SetQuality(int qualityIndex)
    {
        //Pass in int that corresponds to a qualtiy level in Unity
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void Update()
    {
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
        GameObject.Find("HUD/Cursor").GetComponent<Image>().enabled = true;
        if (tutorialUI != null) tutorialUI.SetActive(false);
        InputManager.CurrentInputMode = _lastInputMode;
        GamePaused = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        GameObject.Find("HUD/Cursor").GetComponent<Image>().enabled = false;
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