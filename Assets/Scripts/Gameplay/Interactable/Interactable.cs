using Mirror;
using UnityEngine;
using UnityEngine.Events;

using static InputManager;
using static ButtonPrompt;
using static Holdable;

public abstract class Interactable : NetworkBehaviour
{
    protected UnityEvent _unityEvent = new UnityEvent();

    [SerializeField] private PromptInfo _promptInfo;
    [SerializeField] public Type requiredObject = Type.NONE;

    [SyncVar] private bool _isInteractable = true;

    public PromptInfo PromptInfo { get => _promptInfo; set => _promptInfo = value; }

    public bool HasFocus { get; private set; }

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

    private void Awake()
    {
    }

    protected virtual void Update()
    {
        //Must in focus mode and is interactable
        if (!HasFocus || !IsInteractable) return;

        //can only interacted by player
        if (CurrentInputMode != InputMode.Player) return;

        //Interact
        if (IsCommandJustPressed(PromptInfo.Command)) Interact();
    }

    private void Interact()
    {
        Debug.Log("Interact");

        _unityEvent?.Invoke();
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
            EventManager<PromptInfo>.TriggerEvent("PromptHit", PromptInfo);
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

    #region network

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

    #endregion network
}