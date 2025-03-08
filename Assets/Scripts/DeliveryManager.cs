using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour {

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    
    public static DeliveryManager Instance { get; private set; }
    
    [SerializeField] private RecipeListSO recipeListScriptableObject;
    
    private List<RecipeSO> _waitingRecipeScriptableObjectList;
    private float _spawnRecipeTimer;
    private const float SpawnRecipeTimerMax = 4f;
    private const int WaitingRecipeMax = 4;
    private int _successfulRecipesAmount;

    private void Awake() {
        Instance = this;
        _waitingRecipeScriptableObjectList = new List<RecipeSO>();
    }

    private void Update() {
        _spawnRecipeTimer -= Time.deltaTime;
        if (_spawnRecipeTimer <= 0f) {
            _spawnRecipeTimer = SpawnRecipeTimerMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && _waitingRecipeScriptableObjectList.Count < WaitingRecipeMax) {
                RecipeSO waitingRecipeScriptableObject =
                    recipeListScriptableObject.recipeScriptableObjectList[Random.Range(0, recipeListScriptableObject.recipeScriptableObjectList.Count)];
                
                _waitingRecipeScriptableObjectList.Add(waitingRecipeScriptableObject);
                
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for (var i = 0; i < _waitingRecipeScriptableObjectList.Count; ++i) {
            RecipeSO waitingRecipeScriptableObject = _waitingRecipeScriptableObjectList[i];
            
            if (waitingRecipeScriptableObject.kitchenObjectScriptableObjectList.Count ==
                plateKitchenObject.GetKitchenObjectScriptableObjectList().Count) {
                // Has the same number of ingredients
                bool plateContentsMatchesRecipe = true;
                foreach (var recipeKitchenObjectScriptableObject in waitingRecipeScriptableObject.kitchenObjectScriptableObjectList) {
                    // Cycling through all ingredients in the recipe
                    bool ingredientFound = false;
                    foreach (var plateKitchenObjectScriptableObject in plateKitchenObject.GetKitchenObjectScriptableObjectList()) {
                        // Cycling through all ingredients in the plate
                        if (plateKitchenObjectScriptableObject == recipeKitchenObjectScriptableObject) {
                            // Ingredient matches
                            ingredientFound = true;
                            break;
                        }
                    }

                    if (!ingredientFound) {
                        // This recipe ingredient was not found on the plate
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe) {
                    // Player delivered the correct recipe!

                    ++_successfulRecipesAmount;
                    
                    _waitingRecipeScriptableObjectList.RemoveAt(i);
                    
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }
        // No matches found!
        // Player did not deliver a correct recipe
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeScriptableObjectList() {
        return _waitingRecipeScriptableObjectList;
    }

    public int GetSuccessfulRecipesAmount() {
        return _successfulRecipesAmount;
    }
}
