using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    //[SerializeField] private GameObject _gamepadIndicator;

    [SerializeField] private ButtonPrompt _buttonPrompt;
    private ButtonPrompt.PromptInfo _currentPromptInfo;

    [SerializeField] private Text _timer;
    [SerializeField] private Text _stress;
    [SerializeField] private Text _submitted;
    [SerializeField] private Text _scanned;
    [SerializeField] private Text _written;
    [SerializeField] private Image _stressBarFillImage;
    [SerializeField] private Text _notificationText;

    public static HUD Instance;

    [SerializeField] private Fixable warmPlant;
    [SerializeField] private Fixable coolPlant;
    [SerializeField] private Fixable airConditioning;
    [SerializeField] private CatAgent cat;

    [SerializeField] private GameObject[] stressLevelIcons;
    private int _currentStressLevel = 0;

    private Color _stressBarStartColour;
    private Color _stressBarEndColour;

    private void Start()
    {
        Instance = this;
        EventManager<ButtonPrompt.PromptInfo>.AddListener("PromptHit", OnPromptHit);
        EventManager<ButtonPrompt.PromptInfo>.AddListener("PromptLost", OnPromptLost);

        ButtonIconManager.Init();

        RefreshPlayerIcons();
    }

    public void RefreshPlayerIcons()
    {
        // Using Find here is probably okay bc it only gets called once
        foreach (var icon in stressLevelIcons)
        {
            // Set true first to be sure that Find works
            icon.SetActive(true);
            icon.transform.Find("Warm").gameObject.SetActive(Player.Instance.colour == PlayerCustomization.COLOUR.WARM);
            icon.transform.Find("Cool").gameObject.SetActive(Player.Instance.colour == PlayerCustomization.COLOUR.COOL);
            icon.SetActive(false);
        }
        stressLevelIcons[_currentStressLevel].SetActive(true);

        _stressBarStartColour = Player.Instance.colour == PlayerCustomization.COLOUR.WARM ? new Color(1, 0.9f, 0f) : Color.cyan;
        _stressBarEndColour = Player.Instance.colour == PlayerCustomization.COLOUR.WARM ? Color.red : new Color(1f, 0f, 1f);
    }

    private void OnDestroy()
    {
        EventManager<ButtonPrompt.PromptInfo>.RemoveListener("PromptHit", OnPromptHit);
        EventManager<ButtonPrompt.PromptInfo>.RemoveListener("PromptLost", OnPromptLost);
    }

    private void OnPromptHit(ButtonPrompt.PromptInfo promptInfo)
    {
        _currentPromptInfo = promptInfo;
        _buttonPrompt.Init(promptInfo);
        _buttonPrompt.gameObject.SetActive(true);
    }

    private void OnPromptLost(ButtonPrompt.PromptInfo promptInfo)
    {
        if(_currentPromptInfo.Equals(promptInfo))
        {
            _buttonPrompt.gameObject.SetActive(false);
            _currentPromptInfo = default;
        }
    }

    public void SetGameState(int time, float stress, int submitted, int scanned, int written)
    {
        var minutes = (int)Math.Floor(time / 60f);
        var seconds = (int)(time % 60);

        var minutesString = minutes > 0 ? $"{minutes}:" : "";

        // If there are minutes on the clock we want the seconds to be like 0x if x < 10.
        var secondsString = (minutes > 0 && seconds < 10) ? $"0{seconds}" : $"{seconds}";

        _timer.text = $"{minutesString}{secondsString}";
        SetStressValue(stress);
        _submitted.text = $"Submitted: {submitted}";
        _scanned.text = $"Scanned: {scanned - submitted}";
        _written.text = $"Written: {written}";

        // Notifications
        var notification = "";
        if(stress > 50)
        {
            notification += "Find a way to calm down!\n";
        }
        
        // Tell them what main task to do next
        if(scanned > submitted)
        {
            if (scanned - 1 > submitted) notification += "Upload your assignments!\n";
            else notification += "Upload your assignment!\n";
        }
        else if(scanned == submitted)
        {
            notification += "Write and scan something!\n";
        }

        // Tell them what is causing stress
        var socks = cat.GetNumberOfSocks();
        if(socks != 0)
        {
            notification += $"Put away {socks} sock{(socks == 1 ? "" : "s")}\n";
        }
        if(airConditioning.IsBroken)
        {
            notification += "The air conditioning broke!\n";
        }

        if(warmPlant.IsBroken && coolPlant.IsBroken)
        {
            notification += "Both plants need water!\n";
        }
        else
        {
            if (warmPlant.IsBroken)
            {
                if (Player.Instance.colour == PlayerCustomization.COLOUR.WARM)
                {
                    notification += "Your plant needs water!\n";
                }
                else
                {
                    notification += $"{Player.OtherPlayer?.displayName}'s plant needs water!\n";
                }
            }
            if (coolPlant.IsBroken)
            {
                if (Player.Instance.colour == PlayerCustomization.COLOUR.COOL)
                {
                    notification += "Your plant needs water!\n";
                }
                else
                {
                    notification += $"{Player.OtherPlayer?.displayName ?? "???"}'s plant needs water!\n";
                }
            }
        }

        _notificationText.text = notification;

        // Stress icon
        var expectedStressLevel = Mathf.RoundToInt((stressLevelIcons.Length-1) * stress / 100f);
        if (expectedStressLevel >= stressLevelIcons.Length) expectedStressLevel = stressLevelIcons.Length - 1;
        if (expectedStressLevel < 0) expectedStressLevel = 0;
        if (expectedStressLevel != _currentStressLevel)
        {
            ChangeStressLevel(expectedStressLevel);
        }
    }

    public void SetStressValue(float stress)
    {
        _stress.text = $"Stress: {(int)stress}";
        float pct = stress / 100f;
        _stressBarFillImage.fillAmount = pct;
        _stressBarFillImage.color = Color.Lerp(_stressBarStartColour, _stressBarEndColour, pct);
    }

    public void ChangeStressLevel(int newLevel)
    {
        stressLevelIcons[_currentStressLevel].SetActive(false);
        _currentStressLevel = newLevel;

        stressLevelIcons[_currentStressLevel].SetActive(true);
    }
}