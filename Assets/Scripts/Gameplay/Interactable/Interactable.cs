using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static InputManager;
using static ButtonPrompt;
using UnityEngine.Rendering.Universal;

public abstract class Interactable : NetworkBehaviour
{
    protected UnityEvent _unityEvent = new UnityEvent();

    protected abstract InputCommand InputCommand { get; }

    [SerializeField] private string _promptText;
    [SerializeField] private string _promptTextNotInteractable;
    [SerializeField] private string _promptTextWrongItem;
    [SerializeField] private int _promptPriority;
    [SerializeField] private float _promptHoldTime;

    [SerializeField] public Holdable.Type requiredObject = Holdable.Type.NONE;

    public PromptInfo InteractablePrompt { get; set; }
    public PromptInfo WrongItemPrompt { get; set; }
    public PromptInfo NonInteractablePrompt { get; set; }
    private PromptInfo _lastPrompt;

    [SyncVar] private bool _isInteractable = true;

    public bool HasFocus { get; private set; }

    // If we reset the IsInteractable status back to true when the minigame is done.
    public bool resetAfterUse = false;

    [SerializeField] public string interactionEvent = null;

    private void Awake()
    {
        InteractablePrompt = new PromptInfo(InputCommand, _promptText, _promptPriority, _promptHoldTime);
        WrongItemPrompt = new PromptInfo(InputCommand.None, _promptTextWrongItem, _promptPriority, _promptHoldTime);
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
        var heldObjectType = Player.Instance.heldObject?.type ?? Holdable.Type.NONE;

        return requiredObject == heldObjectType;
    }

    private void Interact()
    {
        Debug.Log($"Interact with object {gameObject.name}");

        // Do nothing if we don't meet the requirements
        if (!IsInteractable || !HasItem()) return;

        if (_unityEvent != null) _unityEvent.Invoke();
    }

    public void GainFocus()
    {
        if (HasFocus) return;

        if (!_isInteractable)
        {
            EventManager<PromptInfo>.TriggerEvent("PromptHit", NonInteractablePrompt);
            _lastPrompt = NonInteractablePrompt;
        }
        else if (!HasItem())
        {
            EventManager<PromptInfo>.TriggerEvent("PromptHit", WrongItemPrompt);
            _lastPrompt = WrongItemPrompt;
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
                Player.Instance.DoWithAuthority(netIdentity, () => CmdSetInteractable(value));
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

        if (HasFocus)
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
