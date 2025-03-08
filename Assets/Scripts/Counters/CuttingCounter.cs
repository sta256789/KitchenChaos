using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CuttingCounter : BaseCounter, IHasProgress {

    public static event EventHandler OnAnyCut;

    public new static void ResetStaticData() {
        OnAnyCut = null;
    }
    
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;
    
    [SerializeField] private CuttingRecipeSO[] cutKitchenObjectScriptableObjectArray;

    private int _cuttingProgress;
    
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // There is no kitchenObject here
            if (player.HasKitchenObject()) {
                // Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    // Player carrying something that can be cut
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    _cuttingProgress = 0;
                    CuttingRecipeSO cuttingRecipeScriptableObject = GetCuttingRecipeScriptableObjectWithInput(GetKitchenObject().GetKitchenObjectSO());
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {ProgressNormalized = (float)_cuttingProgress / cuttingRecipeScriptableObject.cuttingProgressMax});
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
                    }
                }
            }
            else {
                // Player is not carrying something
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player) {
        if (HasKitchenObject() && HasRecipeWithInput((GetKitchenObject().GetKitchenObjectSO()))) {
            // There is a kitchenObject here AND it can be cut
            ++_cuttingProgress;
            
            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            
            CuttingRecipeSO cuttingRecipeScriptableObject = GetCuttingRecipeScriptableObjectWithInput((GetKitchenObject().GetKitchenObjectSO()));
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {ProgressNormalized = (float)_cuttingProgress / cuttingRecipeScriptableObject.cuttingProgressMax});
            
            if (_cuttingProgress >= cuttingRecipeScriptableObject.cuttingProgressMax) {
                KitchenObjectSO outputKitchenObjectScriptObject = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(outputKitchenObjectScriptObject, this);
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectScriptableObject) {
        CuttingRecipeSO cuttingRecipeScriptableObject = GetCuttingRecipeScriptableObjectWithInput(inputKitchenObjectScriptableObject);
        return cuttingRecipeScriptableObject != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectScriptableObject) {
        CuttingRecipeSO cuttingRecipeScriptableObject = GetCuttingRecipeScriptableObjectWithInput(inputKitchenObjectScriptableObject);
        if (cuttingRecipeScriptableObject != null) {
            return cuttingRecipeScriptableObject.output;
        }
        else {
            return null;
        }
    }

    private CuttingRecipeSO GetCuttingRecipeScriptableObjectWithInput(KitchenObjectSO inputKitchenObjectScriptableObject) {
        foreach (var cuttingRecipeScriptableObject in cutKitchenObjectScriptableObjectArray) {
            if (cuttingRecipeScriptableObject.input == inputKitchenObjectScriptableObject) {
                return cuttingRecipeScriptableObject;
            }
        }
        return null;
    }
}
