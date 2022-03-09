using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class Interactable : NetworkBehaviour
{
    protected UnityEvent _event = new UnityEvent();

    [SerializeField] private InputManager.InputCommand _inputCommand;
    [SerializeField] private string _promptText;
    [SerializeField] private int _promptPriority;
    [SerializeField] private float _promptHoldTime;

    [SerializeField] private Holdable.Type _requiredObject = Holdable.Type.NONE;

    public ButtonPrompt.PromptInfo PromptInfo { get; private set; }

    [SyncVar] private bool _isInteractable;

    public bool HasFocus { get; private set; }

    private void Awake()
    {
        _isInteractable = true;
        PromptInfo = new ButtonPrompt.PromptInfo(_inputCommand, _promptText, _promptPriority, _promptHoldTime);
    }

    protected void Update()
    {
        if (!HasFocus || !IsInteractable) return;

        if (InputManager.CurrentInputMode != InputManager.InputMode.Player) return;

        if (InputManager.IsCommandJustPressed(PromptInfo.Command)) Interact();
    }

    private void Interact()
    {
        Debug.Log("Interact");

        if(_requiredObject != Holdable.Type.NONE)
        {
            // Try consuming the required object after use
            Player.Instance.heldObject.Consume();
        }

        if(_event != null) _event.Invoke();
    }

    public void GainFocus()
    {
        if (HasFocus) return;

        // Never gain focus if it requires an object that isn't held
        if(_requiredObject != Holdable.Type.NONE)
        {
            if(Player.Instance.heldObject == null || Player.Instance.heldObject.type != _requiredObject)
            {
                return;
            }
        }

        if(_isInteractable)
        {
            EventManager<ButtonPrompt.PromptInfo>.TriggerEvent("PromptHit", PromptInfo);
        }
        HasFocus = true;
    }

    public void LoseFocus()
    {
        if (!HasFocus) return;

        if (_isInteractable)
        {
            EventManager<ButtonPrompt.PromptInfo>.TriggerEvent("PromptLost", PromptInfo);
        }
        HasFocus = false;
    }

    public bool IsInteractable
    {
        get { return _isInteractable; }
        set
        {
            if (_isInteractable == value) return;

            _isInteractable = value;

            if (!_isInteractable && HasFocus)
            {
                EventManager<ButtonPrompt.PromptInfo>.TriggerEvent("PromptLost", PromptInfo);
            }
            else if (_isInteractable && HasFocus)
            {
                EventManager<ButtonPrompt.PromptInfo>.TriggerEvent("PromptHit", PromptInfo);
            }
        }
    }
}
