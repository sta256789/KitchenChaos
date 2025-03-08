using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour {
    
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamepadInteractButton;
    [SerializeField] private Button gamepadInteractAlternateButton;
    [SerializeField] private Button gamepadPauseButton;
    
    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAlternateText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI gamepadInteractText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAlternateText;
    [SerializeField] private TextMeshProUGUI gamepadPauseText;

    [SerializeField] private Transform pressToRebindKeyTransform;

    private Action _onCloseButtonAction;

    private void Awake() {
        Instance = this;
        
        soundEffectsButton.onClick.AddListener(() => {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() => {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        
        closeButton.onClick.AddListener(() => {
            Hide();
            _onCloseButtonAction();
        });
        
        moveUpButton.onClick.AddListener(() => {RebindBinding(GameInput.Binding.MoveUp);});
        moveDownButton.onClick.AddListener(() => {RebindBinding(GameInput.Binding.MoveDown);});
        moveLeftButton.onClick.AddListener(() => {RebindBinding(GameInput.Binding.MoveLeft);});
        moveRightButton.onClick.AddListener(() => {RebindBinding(GameInput.Binding.MoveRight);});
        interactButton.onClick.AddListener(() => {RebindBinding(GameInput.Binding.Interact);});
        interactAlternateButton.onClick.AddListener(() => {RebindBinding(GameInput.Binding.InteractAlternate);});
        pauseButton.onClick.AddListener(() => {RebindBinding(GameInput.Binding.Pause);});
        gamepadInteractButton.onClick.AddListener(() => {RebindBinding(GameInput.Binding.GamepadInteract);});
        gamepadInteractAlternateButton.onClick.AddListener(() => {RebindBinding(GameInput.Binding.GamepadInteractAlternate);});
        gamepadPauseButton.onClick.AddListener(() => {RebindBinding(GameInput.Binding.GamepadPause);});
    }

    private void Start() {
        KitchenGameManager.Instance.OnGamePaused += KitchenGameManager_OnGameUnpaused;
        
        UpdateVisual();
        
        Hide();
        HidePressToRebindKey(); 
    }

    private void KitchenGameManager_OnGameUnpaused(object sender, EventArgs e) {
        Hide();
    }

    private void UpdateVisual() {
        soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        moveUpText.text = GameInput.Instance.GetBidingText(GameInput.Binding.MoveUp);
        moveDownText.text = GameInput.Instance.GetBidingText(GameInput.Binding.MoveDown);
        moveLeftText.text = GameInput.Instance.GetBidingText(GameInput.Binding.MoveLeft);
        moveRightText.text = GameInput.Instance.GetBidingText(GameInput.Binding.MoveRight);
        interactText.text = GameInput.Instance.GetBidingText(GameInput.Binding.Interact);
        interactAlternateText.text = GameInput.Instance.GetBidingText(GameInput.Binding.InteractAlternate);
        pauseText.text = GameInput.Instance.GetBidingText(GameInput.Binding.Pause);
        gamepadInteractText.text = GameInput.Instance.GetBidingText(GameInput.Binding.GamepadInteract);
        gamepadInteractAlternateText.text = GameInput.Instance.GetBidingText(GameInput.Binding.GamepadInteractAlternate);
        gamepadPauseText.text = GameInput.Instance.GetBidingText(GameInput.Binding.GamepadPause);
    }

    public void Show(Action onCloseButtonAction) {
        this._onCloseButtonAction = onCloseButtonAction;
        
        gameObject.SetActive(true);
        
        soundEffectsButton.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey() {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }
    
    private void HidePressToRebindKey() {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInput.Binding binding) {
        ShowPressToRebindKey();
        GameInput.Instance.RebindBinding(binding, () => {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }
}
