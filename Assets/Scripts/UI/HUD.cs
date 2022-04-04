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
    [SerializeField] private Image _stressBarFillImage;

    public static HUD Instance;
    private static GameDirector _director;

    private void Start()
    {
        Instance = this;
        _director = GameDirector.Instance;
        EventManager<ButtonPrompt.PromptInfo>.AddListener("PromptHit", OnPromptHit);
        EventManager<ButtonPrompt.PromptInfo>.AddListener("PromptLost", OnPromptLost);

        ButtonIconManager.Init();
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

    public void SetGameState(int time, float stress, int submitted, int scanned)
    {
        var minutes = (int)Math.Floor(time / 60f);
        var seconds = (int)(time % 60);

        var minutesString = minutes > 0 ? $"{minutes}:" : "";

        // If there are minutes on the clock we want the seconds to be like 0x if x < 10.
        var secondsString = (minutes > 0 && seconds < 10) ? $"0{seconds}" : $"{seconds}";

        _timer.text = $"{minutesString}{secondsString}";
        SetStressValue(stress);
        _submitted.text = $"Submitted: {submitted}";
        _scanned.text = $"Scanned: {scanned}";
    }

    public void SetStressValue(float stress)
    {
        _stress.text = $"Stress: {(int)stress}";
        float pct = stress / 100f;
        _stressBarFillImage.fillAmount = pct;
        _stressBarFillImage.color = Color.Lerp(Color.green, Color.red, pct);
    }
}