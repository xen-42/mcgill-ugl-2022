using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionSettingsLoader : MonoBehaviour
{
    public OptionSettings settings;

    public float a;
    public float b;
    public float c;

    private void Start()
    {
        settings.OnLoad();
    }
}