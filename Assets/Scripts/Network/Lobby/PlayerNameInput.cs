using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField = null;

    public static string DisplayName { get; private set; }

    private const string PlayerPrefsNameKey = "PlayerName";


    void Start()
    {
        string defaultName = "Player";

        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey))
        {
            // Try-catch in case steam works isnt initialized
            try
            {
                defaultName = SteamFriends.GetPersonaName();
            }
            catch { }

            return;
        }
        else
        {
            defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
        }

        nameInputField.text = defaultName;
        DisplayName = defaultName;
    }

    public void SavePlayerName()
    {
        if(string.IsNullOrWhiteSpace(nameInputField.text))
        {
            nameInputField.text = "Player";
        }

        DisplayName = nameInputField.text;
        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(Scenes.MainMenu);
    }

    public void OnTutorialButtonPressed(){
        SceneManager.LoadScene(Scenes.Tutorial);
    }
}
