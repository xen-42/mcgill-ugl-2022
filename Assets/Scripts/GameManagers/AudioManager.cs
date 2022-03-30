using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public AudioSource petCat;
    
    /*
    void Awake()
    {
        foreach (Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatialBlend;
            s.source.playOnAwake = s.playOnAwake;
            s.source.loop = s.loop;
        }
    }
    
    public void PlaySound(string name){
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        sound.source.Play();
    }

    public void StopSound(string name){
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        sound.source.Stop();
    }

    public void ChangeVolume(string name, float new_volume){
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        sound.source.volume = new_volume;
        sound.volume = new_volume;
    }

    public float GetVolume(string name){
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        return sound.source.volume;
    }*/
}
