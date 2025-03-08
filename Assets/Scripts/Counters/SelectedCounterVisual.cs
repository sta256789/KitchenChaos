using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectedCounterVisual : MonoBehaviour {
    
    [FormerlySerializedAs("clearCounter")] [SerializeField] private BaseCounter baseCounter;
    [FormerlySerializedAs("visualGameObject")] [SerializeField] private GameObject[] visualGameObjectArray;
    
    private void Start() {
        Player.Instance.OnSelectedCounterChanged += PlayerOnSelectedCounterChanged;
    }

    private void PlayerOnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e) {
        if (e.SelectedCounter == baseCounter) {
            Show();
        }
        else {
            Hide();
        }
    }

    private void Show() {
        foreach (var visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(true); 
        }
    }
    
    private void Hide() {
        foreach (var visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(false); 
        }
    }
}
