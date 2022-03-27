using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound {
    
    // Audio clip 
    public AudioClip clip;
    public string name;

    [HideInInspector]
    public AudioSource source;

    // Sound parameters
    [Range(0f, 1f)]
    public float volume = 0.5f;

    [Range(.1f, 3f)]
    public float pitch = 1.0f;

    [HideInInspector] public float spatialBlend = 1.0f;

    [HideInInspector] public bool playOnAwake = false;

}
