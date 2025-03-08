using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI keyMoveUpText;
    [SerializeField] private TextMeshProUGUI keyMoveDownText;
    [SerializeField] private TextMeshProUGUI keyMoveLeftText;
    [SerializeField] private TextMeshProUGUI keyMoveRightText;
    [SerializeField] private TextMeshProUGUI keyInteractText;
    [SerializeField] private TextMeshProUGUI keyInteractAlternateText;
    [SerializeField] private TextMeshProUGUI keyPauseText;
    [SerializeField] private TextMeshProUGUI keyGamepadInteractText;
    [SerializeField] private TextMeshProUGUI keyGamepadInteractAlternateText;
    [SerializeField] private TextMeshProUGUI keyGamepadPauseText;

    private void Start() {
        GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        UpdateVisual();
        Show();
    }

    private void KitchenGameManager_OnStateChanged(object sender, EventArgs e) {
        if (KitchenGameManager.Instance.IsCountdownToStartActive()) {
            Hide();
        }
    }

    private void GameInput_OnBindingRebind(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        keyMoveUpText.text = GameInput.Instance.GetBidingText(GameInput.Binding.MoveUp);
        keyMoveDownText.text = GameInput.Instance.GetBidingText(GameInput.Binding.MoveDown);
        keyMoveLeftText.text = GameInput.Instance.GetBidingText(GameInput.Binding.MoveLeft);
        keyMoveRightText.text = GameInput.Instance.GetBidingText(GameInput.Binding.MoveRight);
        keyInteractText.text = GameInput.Instance.GetBidingText(GameInput.Binding.Interact);
        keyInteractAlternateText.text = GameInput.Instance.GetBidingText(GameInput.Binding.InteractAlternate);
        keyPauseText.text = GameInput.Instance.GetBidingText(GameInput.Binding.Pause);
        keyGamepadInteractText.text = GameInput.Instance.GetBidingText(GameInput.Binding.GamepadInteract);
        keyGamepadInteractAlternateText.text = GameInput.Instance.GetBidingText(GameInput.Binding.GamepadInteractAlternate);
        keyGamepadPauseText.text = GameInput.Instance.GetBidingText(GameInput.Binding.GamepadPause);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
