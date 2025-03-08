using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {

    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
    
    public static GameInput Instance { get; private set; }

    public event EventHandler OnBindingRebind;
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    public enum Binding {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternate,
        Pause,
        GamepadInteract,
        GamepadInteractAlternate,
        GamepadPause,
    }
    
    private PlayerInputActions _playerInputActions;
    private void Awake() {
        Instance = this;
        
        _playerInputActions = new PlayerInputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS)) {
            _playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        
        _playerInputActions.Player.Enable();

        _playerInputActions.Player.Interact.performed += Interact_Performed;
        _playerInputActions.Player.InteractAlternate.performed += InteractAlternate_Performed;
        _playerInputActions.Player.Pause.performed += Pause_Performed;
    }

    private void OnDestroy() {
        _playerInputActions.Player.Interact.performed -= Interact_Performed;
        _playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_Performed;
        _playerInputActions.Player.Pause.performed -= Pause_Performed;
        
        _playerInputActions.Dispose();
    }

    private void Pause_Performed(InputAction.CallbackContext obj) {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_Performed(InputAction.CallbackContext obj) {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_Performed(InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {
        
        var inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
        
        return inputVector.normalized;
    }

    public string GetBidingText(Binding binding) {
        switch (binding) {
            case Binding.MoveUp:
                return _playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.MoveDown:
                return _playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.MoveLeft:
                return _playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.MoveRight:
                return _playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return _playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return _playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return _playerInputActions.Player.Pause.bindings[0].ToDisplayString();
            case Binding.GamepadInteract:
                return _playerInputActions.Player.Interact.bindings[1].ToDisplayString();
            case Binding.GamepadInteractAlternate:
                return _playerInputActions.Player.InteractAlternate.bindings[1].ToDisplayString();
            case Binding.GamepadPause:
                return _playerInputActions.Player.Pause.bindings[1].ToDisplayString();
            default:
                throw new ArgumentOutOfRangeException(nameof(binding), binding, null);
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound) {
        _playerInputActions.Player.Disable();

        InputAction inputAction;
        int bindingIndex;

        switch (binding) {
            case Binding.MoveUp:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.MoveDown:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.MoveLeft:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.MoveRight:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = _playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = _playerInputActions.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = _playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
            case Binding.GamepadInteract:
                inputAction = _playerInputActions.Player.Interact;
                bindingIndex = 1;
                break;
            case Binding.GamepadInteractAlternate:
                inputAction = _playerInputActions.Player.InteractAlternate;
                bindingIndex = 1;
                break;
            case Binding.GamepadPause:
                inputAction = _playerInputActions.Player.Pause;
                bindingIndex = 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(binding), binding, null);
        }

        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback => {
            // Just in case
            callback.Dispose();
            
            _playerInputActions.Player.Enable();
            onActionRebound();
            
            PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, _playerInputActions.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
            
            OnBindingRebind?.Invoke(this, EventArgs.Empty);
        }).Start();
    }
}
