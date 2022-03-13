using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class Interactable : NetworkBehaviour
{
    protected UnityEvent _unityEvent = new UnityEvent();

    [SerializeField] private InputManager.InputCommand _inputCommand;
    [SerializeField] private string _promptText;
    [SerializeField] private int _promptPriority;
    [SerializeField] private float _promptHoldTime;

    [SerializeField] public Holdable.Type requiredObject = Holdable.Type.NONE;

    public ButtonPrompt.PromptInfo PromptInfo { get; set; }

    [SyncVar] private bool _isInteractable = true;

    public bool HasFocus { get; private set; }

    private void Awake()
    {
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

        if (_unityEvent != null) _unityEvent.Invoke();
    }

    public void GainFocus()
    {
        if (HasFocus) return;

        // Never gain focus if it requires an object that isn't held
        if (requiredObject != Holdable.Type.NONE)
        {
            if (Player.Instance.heldObject == null || Player.Instance.heldObject.type != requiredObject)
            {
                return;
            }
        }

        if (_isInteractable)
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
        get
        {
            return _isInteractable;
        }
        set
        {
            if (!isServer)
            {
                // Need authority before we can give commands
                Player.Instance.CmdGiveAuthority(netIdentity);
                ActionManager.RunWhen(() => netIdentity.hasAuthority, () => CmdSetInteractable(value));
            }
            else
            {
                RpcSetInteractable(value);
            }
        }
    }


    [Command]
    private void CmdSetInteractable(bool value)
    {
        RpcSetInteractable(value);
    }

    [ClientRpc]
    private void RpcSetInteractable(bool value)
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
