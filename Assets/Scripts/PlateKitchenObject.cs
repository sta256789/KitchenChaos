using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO KitchenObjectScriptableObject;
    }
    
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectScriptableObjectList;

    private List<KitchenObjectSO> _kitchenObjectScriptableObjectList;

    private void Awake() {
        _kitchenObjectScriptableObjectList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectScriptableObject) {
        if (!validKitchenObjectScriptableObjectList.Contains(kitchenObjectScriptableObject)) {
            // Not a valid ingredient
            return false;
        }
        if (_kitchenObjectScriptableObjectList.Contains(kitchenObjectScriptableObject)) {
            // Already has this type
            return false;
        }
        else {
            _kitchenObjectScriptableObjectList.Add(kitchenObjectScriptableObject);
            
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs {KitchenObjectScriptableObject = kitchenObjectScriptableObject});
            return true;
        }
    }

    public List<KitchenObjectSO> GetKitchenObjectScriptableObjectList() {
        return _kitchenObjectScriptableObjectList;
    }
    
}
