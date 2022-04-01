using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelector : MonoBehaviour
{
    [SerializeField] public GameObject[] options;
    [SerializeField] public Button leftButton;
    [SerializeField] public Button rightButton;
    [SerializeField] public string optionID;

    public int Selection { get; private set; }

    void Awake()
    {
        if(PlayerPrefs.HasKey(optionID))
        {
            Selection = PlayerPrefs.GetInt(optionID);
        }

        for(int i = 0; i < options.Length; i++)
        {
            options[i].gameObject.SetActive(i == Selection);
        }

        leftButton.onClick.AddListener(OnLeftButtonClick);
        rightButton.onClick.AddListener(OnRightButtonClick);
    }

    void OnDestroy()
    {
        leftButton.onClick.RemoveListener(OnLeftButtonClick);
        rightButton.onClick.RemoveListener(OnRightButtonClick);
    }

    private void OnLeftButtonClick()
    {
        SetSelection(Posmod(Selection + 1, options.Length));
    }

    private void OnRightButtonClick()
    {
        SetSelection(Posmod(Selection - 1, options.Length));
    }

    private void SetSelection(int newSelection)
    {
        Debug.Log($"Picking option [{newSelection}] out of [{options.Length}]");

        options[Selection].gameObject.SetActive(false);
        options[newSelection].gameObject.SetActive(true);

        Selection = newSelection;

        PlayerPrefs.SetInt(optionID, Selection);
    }

    private int Posmod(int x, int m)
    {
        x = x % m;
        if (x < 0) x += m;
        return x;
    }
}
