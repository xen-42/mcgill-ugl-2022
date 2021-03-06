using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "OptionSettings")]
public class OptionSettings : ScriptableObject
{
    #region Gameplay Settings

    [SerializeField] [Range(10f, 50f)] private float m_cameraSensitivity;

    public float CameraSensitivity
    {
        get => m_cameraSensitivity;
        set => Player.sensX = Player.sensY = m_cameraSensitivity = value;
    }

    #endregion Gameplay Settings

    #region Graphics Settings

    private int m_resolution;

    #endregion Graphics Settings

    #region Sound Settings

    [SerializeField] private AudioMixer m_masterAudioMixer;
    [SerializeField] private AudioMixer m_musicAudioMixer;
    [SerializeField] private AudioMixer m_soundAudioMixer;
    [SerializeField] [Range(.0001f, 1f)] private float m_masterAudioVolume = 1f;
    [SerializeField] [Range(.0001f, 1f)] private float m_musicAudioVolume = 1f;
    [SerializeField] [Range(.0001f, 1f)] private float m_soundAudioVolume = 1f;

    public AudioMixer MasterAudioMixer => m_masterAudioMixer;
    public AudioMixer MusicAudioMixer => m_musicAudioMixer;
    public AudioMixer SoundAudioMixer => m_soundAudioMixer;
    public float MasterVolume { get => m_masterAudioVolume; set => m_masterAudioMixer.SetFloat("volume", 20 * Mathf.Log10(m_masterAudioVolume = value)); }
    public float MusicVolume { get => m_musicAudioVolume; set => m_musicAudioMixer.SetFloat("volume", 20 * Mathf.Log10(m_musicAudioVolume = value)); }
    public float SoundVolume { get => m_soundAudioVolume; set => m_soundAudioMixer.SetFloat("volume", 20 * Mathf.Log10(m_soundAudioVolume = value)); }

    #endregion Sound Settings

    public void OnLoad()
    {
        MasterVolume = m_masterAudioVolume;
        MusicVolume = m_musicAudioVolume;
        SoundVolume = m_soundAudioVolume;
        CameraSensitivity = m_cameraSensitivity;
    }
}