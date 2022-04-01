using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusic : MonoBehaviour
{
    private AudioSource menuMusicTheme;

    private void Awake(){
        DontDestroyOnLoad(transform.gameObject);
        menuMusicTheme = GetComponent<AudioSource>();
    }

    public void Update(){
        if (SceneManager.GetActiveScene().name.Equals("MovementPrototype")){
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
