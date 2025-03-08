using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour {
    
    [Serializable]
    public struct KitchenObjectScriptableObjectGameObject {
        
        public KitchenObjectSO kitchenObjectScriptableObject;
        public GameObject gameObject;
        
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectScriptableObjectGameObject> kitchenObjectScriptableObjectGameObjectList;

    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        
        foreach (var kitchenObjectScriptableObjectGameObject in kitchenObjectScriptableObjectGameObjectList) {
            kitchenObjectScriptableObjectGameObject.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        foreach (var kitchenObjectScriptableObjectGameObject in kitchenObjectScriptableObjectGameObjectList) {
            if (kitchenObjectScriptableObjectGameObject.kitchenObjectScriptableObject ==
                e.KitchenObjectScriptableObject) {
                kitchenObjectScriptableObjectGameObject.gameObject.SetActive(true);
            }
        }
    }
}
