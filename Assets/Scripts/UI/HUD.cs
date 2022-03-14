using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private GameObject _gamepadIndicator;

    [SerializeField] private GameObject _buttonPrompt;

    #region UI Variable Values
    [SerializeField] private Text _timer;
    [SerializeField] private Text _stress;
    [SerializeField] private Text _submitted;
    [SerializeField] private Image _stressBarFillImage;

    #endregion UI Variable Values

    [SerializeField] private Text _gameOverText;

    private Dictionary<ButtonPrompt.PromptInfo, ButtonPrompt> _buttonPromptDict;

    public static HUD Instance;
    private static GameDirector _director;

    private void Start()
    {
        Instance = this;
        _director = GameDirector.Instance;
        EventManager<ButtonPrompt.PromptInfo>.AddListener("PromptHit", OnPromptHit);
        EventManager<ButtonPrompt.PromptInfo>.AddListener("PromptLost", OnPromptLost);

        ButtonIconManager.Init();

        _buttonPromptDict = new Dictionary<ButtonPrompt.PromptInfo, ButtonPrompt>();
    }

    private void OnDestroy()
    {
        EventManager<ButtonPrompt.PromptInfo>.RemoveListener("PromptHit", OnPromptHit);
        EventManager<ButtonPrompt.PromptInfo>.RemoveListener("PromptLost", OnPromptLost);
    }

    private void Update()
    {
        var indicatorShown = _gamepadIndicator.activeInHierarchy;
        var gamepadEnabled = InputManager.IsGamepadEnabled();

        if (indicatorShown == gamepadEnabled)
        {
            _gamepadIndicator.SetActive(!indicatorShown);
        }
    }

    private void OnPromptHit(ButtonPrompt.PromptInfo promptInfo)
    {
        if (_buttonPromptDict.TryGetValue(promptInfo, out ButtonPrompt buttonPrompt))
        {
            buttonPrompt.gameObject.SetActive(true);
        }
        else
        {
            var newPrompt = GameObject.Instantiate(_buttonPrompt, _buttonPrompt.transform.parent);
            newPrompt.GetComponent<ButtonPrompt>().OnInit(promptInfo);

            _buttonPromptDict.Add(promptInfo, newPrompt.GetComponent<ButtonPrompt>());
            newPrompt.SetActive(true);

            SortPrompts();
        }
    }

    private void OnPromptLost(ButtonPrompt.PromptInfo promptInfo)
    {
        if (_buttonPromptDict.TryGetValue(promptInfo, out ButtonPrompt buttonPrompt))
        {
            _buttonPromptDict.Remove(promptInfo);
            GameObject.Destroy(buttonPrompt.gameObject);

            SortPrompts();
        }
    }

    private void SortPrompts()
    {
        // Reorder the remaining ones
        int i = 0;
        var sortedPrompts = _buttonPromptDict.Values.OrderBy(x => x.Info.Priority);
        foreach (var prompt in _buttonPromptDict.Values.ToArray())
        {
            RectTransform rect = prompt.GetComponent<RectTransform>();
            var pos = rect.localPosition;
            pos.y = i++ * -rect.sizeDelta.y;
            rect.localPosition = pos;
        }
    }

    #region Variable Values Update Callbacks

    /// <summary>
    /// Update Time Remaining, the stress level of the player, and how many assignments player has submitted
    /// </summary>
    /// <param name="time"></param>
    /// <param name="stress"></param>
    /// <param name="submitted"></param>
    public void SetGameState(int time, float stress, int submitted)
    {
        if (time < 0)
        {
            _gameOverText.text = $"The semester is over!\n You finished {submitted} assignments.";
            return;
        }

        var minutes = (int)Math.Floor(time / 60f);
        var seconds = (int)(time % 60);

        var minutesString = minutes > 0 ? $"{minutes}:" : "";

        // If there are minutes on the clock we want the seconds to be like 0x if x < 10.
        var secondsString = (minutes > 0 && seconds < 10) ? $"0{seconds}" : $"{seconds}";

        _timer.text = $"Time: {minutesString}{secondsString}";
        SetStressValue(stress);
        _submitted.text = $"Assignments submitted: {submitted}";
    }

    public void SetStressValue(float stress)
    {
        _stress.text = $"Stress: {(int)stress}";
        float pct = stress / 100f;
        _stressBarFillImage.fillAmount = pct;
        _stressBarFillImage.color = Color.Lerp(Color.green, Color.red, pct);
    }

    #endregion Variable Values Update Callbacks
}