using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour{
    public static bool GamePaused = false;
    public GameObject pauseMenuUI;
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    Resolution[] resolutions;
    int currentResolutionIndex = 0;
    public GameObject playercharacter;
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
    public void SetMouseSensitiity(float sens){
        GameObject.Find("Player [connId=0]").GetComponent<CameraController>().sensX = sens;
        GameObject.Find("Player [connId=0]").GetComponent<CameraController>().sensY = sens;

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

    public void Resume(){
        pauseMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        GamePaused = false;
    }

    void Pause(){
        pauseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        GamePaused = true;
    }

    public void LoadMenu(){
        
    }

    public void ReturnToMainMenu(){
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

    }
}
