using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter {
    
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectScriptableObject;
    
    private float _spawnPlateTimer;
    private const float SpawnPlateTimerMax = 4f;
    private int _platesSpawnedAmount;
    private const int PlatesSpawnedAmountMax = 4;

    private void Update() {
        _spawnPlateTimer += Time.deltaTime;
        if (_spawnPlateTimer > SpawnPlateTimerMax) {
            _spawnPlateTimer = 0f;

            if (KitchenGameManager.Instance.IsGamePlaying() && _platesSpawnedAmount < SpawnPlateTimerMax) {
                ++_platesSpawnedAmount;
                
                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    
    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            // Player is empty handed
            if (_platesSpawnedAmount > 0) {
                // There's at least one plate here
                --_platesSpawnedAmount;

                KitchenObject.SpawnKitchenObject(plateKitchenObjectScriptableObject, player);
                
                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
