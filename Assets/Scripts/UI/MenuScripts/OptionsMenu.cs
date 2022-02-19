using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour{
    public static bool GamePaused = false;
    public GameObject pauseMenuUI;
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    Resolution[] resolutions;
    int currentResolutionIndex = 0;
    void Start(){
        //Get array of resolutions
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i =0; i < resolutions.Length; i ++){
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height){
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetVolume(float volume){
        //Sets the game volume
        //Used in the slider
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex){
        //Pass in int that corresponds to a qualtiy level in Unity
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen){
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex){
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

     void Update() {
        //Checks if the game is paused or not
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (GamePaused){
                Resume();
            
            }else{
                Pause();
            }
        }
    }

    void Resume(){
        pauseMenuUI.SetActive(false);
        GamePaused = false;
    }

    void Pause(){
        pauseMenuUI.SetActive(true);
        GamePaused = true;
    }
}
