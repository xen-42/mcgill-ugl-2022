using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static InputManager;
using static ButtonPrompt;

public abstract class Interactable : NetworkBehaviour
{
    protected UnityEvent _unityEvent = new UnityEvent();

    protected abstract InputCommand InputCommand { get; }

    [SerializeField] private string _promptText;
    [SerializeField] private string _promptTextNotInteractable;
    [SerializeField] private string _promptTextMissingItem;
    [SerializeField] private int _promptPriority;
    [SerializeField] private float _promptHoldTime;

    [SerializeField] public Holdable.Type requiredObject = Holdable.Type.NONE;

    public PromptInfo InteractablePrompt { get; set; }
    public PromptInfo MissingItemPrompt { get; set; }
    public PromptInfo NonInteractablePrompt { get; set; }
    private PromptInfo _lastPrompt;


    [SyncVar] private bool _isInteractable = true;

    public bool HasFocus { get; private set; }

    private void Awake()
    {
        InteractablePrompt = new PromptInfo(InputCommand, _promptText, _promptPriority, _promptHoldTime);
        MissingItemPrompt = new PromptInfo(InputCommand.None, _promptTextMissingItem, _promptPriority, _promptHoldTime);
        NonInteractablePrompt = new PromptInfo(InputCommand.None, _promptTextNotInteractable, _promptPriority, _promptHoldTime);
    }

    public void Init(bool initialStatus)
    {
        _isInteractable = initialStatus;
    }

    protected void Update()
    {
        if (!HasFocus || !IsInteractable) return;

        if (CurrentInputMode != InputMode.Player) return;

        if (IsCommandJustPressed(InputCommand)) Interact();
    }

    protected virtual bool HasItem()
    {
        return requiredObject == Holdable.Type.NONE || (Player.Instance.heldObject != null && Player.Instance.heldObject.type == requiredObject);
    }

    private void Interact()
    {
        // Do nothing if we don't meet the requirements
        if (!IsInteractable || !HasItem()) return;

        if (_unityEvent != null) _unityEvent.Invoke();
    }

    public void GainFocus()
    {
        if (HasFocus) return;

        if(!_isInteractable) 
        {
            EventManager<PromptInfo>.TriggerEvent("PromptHit", NonInteractablePrompt);
            _lastPrompt = NonInteractablePrompt;
        }
        else if (!HasItem())
        {
            EventManager<PromptInfo>.TriggerEvent("PromptHit", MissingItemPrompt);
            _lastPrompt = MissingItemPrompt;
        }
        else
        {
            EventManager<PromptInfo>.TriggerEvent("PromptHit", InteractablePrompt);
            _lastPrompt = InteractablePrompt;
        }              

        HasFocus = true;
    }

    public void LoseFocus()
    {
        if (!HasFocus) return;

        EventManager<PromptInfo>.TriggerEvent("PromptLost", _lastPrompt);

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

        if(HasFocus)
        {
            // Remove old prompt
            EventManager<PromptInfo>.TriggerEvent("PromptLost", _lastPrompt);

            if (!_isInteractable)
            {
                // Set new prompt (can't interact)
                EventManager<PromptInfo>.TriggerEvent("PromptHit", NonInteractablePrompt);
                _lastPrompt = NonInteractablePrompt;
            }
            else
            {
                // Set new prompt (can interact)
                EventManager<PromptInfo>.TriggerEvent("PromptHit", InteractablePrompt);
                _lastPrompt = InteractablePrompt;
            }
        }
    }
}
