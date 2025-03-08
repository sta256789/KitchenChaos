using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress {

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs {
        public State CurrentState;
    }
    public enum State {
        Idle,
        Frying,
        Fried,
        Burned,
    }
    
    [SerializeField] private FryingRecipeSO[] fryingRecipeScriptableObjectArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeScriptableObjectArray;

    private float _fryingTimer;
    private float _burningTimer;
    private FryingRecipeSO _fryingRecipeScriptableObject;
    private BurningRecipeSO _burningRecipeScriptableObject;
    private State _currentState;

    private void Start() {
        _currentState = State.Idle;
    }

    private void Update() {
        
        if (HasKitchenObject()) {
            switch (_currentState) {
                case State.Idle:
                    break;
                case State.Frying:
                    _fryingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {ProgressNormalized = _fryingTimer / _fryingRecipeScriptableObject.fryingTimerMax});
                    if (_fryingTimer > _fryingRecipeScriptableObject.fryingTimerMax) {
                        // Fried
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_fryingRecipeScriptableObject.output, this);
                        
                        _burningTimer = 0f;
                        _burningRecipeScriptableObject = GetBurningRecipeScriptableObjectWithInput(GetKitchenObject().GetKitchenObjectSO());
                        
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {CurrentState = _currentState});
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {ProgressNormalized = _fryingTimer / _fryingRecipeScriptableObject.fryingTimerMax});
                        _currentState = State.Fried;
                    }
                    break;
                case State.Fried:
                    _burningTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {ProgressNormalized = _burningTimer / _burningRecipeScriptableObject.burningTimerMax});
                    if (_burningTimer > _burningRecipeScriptableObject.burningTimerMax) {
                        // Fried
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_burningRecipeScriptableObject.output, this);
                        _currentState = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {CurrentState = _currentState});
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {ProgressNormalized = 0f});
                        _currentState = State.Burned;
                    }
                    break;
                case State.Burned:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // There is no kitchenObject here
            if (player.HasKitchenObject()) {
                // Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    // Player carrying something that can be fried
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    _fryingRecipeScriptableObject = GetFryingRecipeScriptableObjectWithInput(GetKitchenObject().GetKitchenObjectSO());
                    _currentState = State.Frying;
                    _fryingTimer = 0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {CurrentState = _currentState});
                }
            }
            else {
                // player not carrying something
            }
        }
        else {
            // There is a kitchenObject here
            if (player.HasKitchenObject()) {
                // Player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    // Player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                        _currentState = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {CurrentState = _currentState});
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {ProgressNormalized = 0f});
                    }
                }
            }
            else {
                // Player is not carrying something
                GetKitchenObject().SetKitchenObjectParent(player);
                _currentState = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {CurrentState = _currentState});
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {ProgressNormalized = 0f});
            }
        }
    }
    
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectScriptableObject) {
        FryingRecipeSO fryingRecipeScriptableObject = GetFryingRecipeScriptableObjectWithInput(inputKitchenObjectScriptableObject);
        return fryingRecipeScriptableObject != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectScriptableObject) {
        FryingRecipeSO fryingRecipeScriptableObject = GetFryingRecipeScriptableObjectWithInput(inputKitchenObjectScriptableObject);
        if (fryingRecipeScriptableObject != null) {
            return fryingRecipeScriptableObject.output;
        }
        else {
            return null;
        }
    }

    private FryingRecipeSO GetFryingRecipeScriptableObjectWithInput(KitchenObjectSO inputKitchenObjectScriptableObject) {
        foreach (var fryingRecipeScriptableObject in fryingRecipeScriptableObjectArray) {
            if (fryingRecipeScriptableObject.input == inputKitchenObjectScriptableObject) {
                return fryingRecipeScriptableObject;
            }
        }
        return null;
    }
    
    private BurningRecipeSO GetBurningRecipeScriptableObjectWithInput(KitchenObjectSO inputKitchenObjectScriptableObject) {
        foreach (var burningRecipeScriptableObject in burningRecipeScriptableObjectArray) {
            if (burningRecipeScriptableObject.input == inputKitchenObjectScriptableObject) {
                return burningRecipeScriptableObject;
            }
        }
        return null;
    }

    public bool IsFried() {
        return _currentState == State.Fried;
    }
}
