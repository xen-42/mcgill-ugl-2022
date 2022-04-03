using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    public bool GamePaused = false;
    public GameObject pauseMenuUI;
    public GameObject tutorialUI;
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;
    public GameObject playercharacter;
    public Slider sensSlider;
    private InputManager.InputMode _lastInputMode = InputManager.InputMode.Player;

    private void Start()
    {
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

            if (sensSlider != null)
            {
                sensSlider.value = (float)(Player.Instance.sensX);
            }
        }

        //pauseMenuUI.SetActive(false);
        //tutorialUI.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        //Sets the game volume
        //Used in the slider
        if (audioMixer.SetFloat("volume", volume))
            print("test");
    }

    public void SetMouseSensitiity(float sens)
    {
        Player.Instance.sensX = sens;
        Player.Instance.sensY = sens;
    }

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
        if (tutorialUI != null) tutorialUI.SetActive(false);
        InputManager.CurrentInputMode = _lastInputMode;
        GamePaused = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
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