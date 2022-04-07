using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusic : MonoBehaviour
{
    private AudioSource menuMusicTheme;

    private static MenuMusic _instance;

    void Start()
    {
        // Only ever have one at a time
        if (_instance != null)
        {
            Debug.Log($"Can only have one {nameof(MenuMusic)}");
            GameObject.Destroy(gameObject);
        }
        else
        {
            _instance = this;

            DontDestroyOnLoad(transform.gameObject);
            menuMusicTheme = GetComponent<AudioSource>();
            Play();
        }
    }

    public void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == Scenes.GameScene)
        {
            _instance = null;
            Destroy(transform.gameObject);
        }
    }

    public void Play()
    {
        if (menuMusicTheme.isPlaying) return;
        menuMusicTheme.Play();
    }

    public void Stop()
    {
        menuMusicTheme.Stop();
    }
}
