using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour {

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    
    public static KitchenGameManager Instance { get; private set; }
    
    private enum GameState {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private GameState _currentState;
    private float _countdownToStartTimer = 3f;
    private float _gamePlayingTimer;
    private const float GamePlayingTimerMax = 90f;
    private bool _isGamePaused = false;

    private void Awake() {
        Instance = this;
        
        _currentState = GameState.WaitingToStart;
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if (_currentState == GameState.WaitingToStart) {
            _currentState = GameState.CountdownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }


    private void Update() {
        switch (_currentState) {
            case GameState.WaitingToStart:
                break;
            case GameState.CountdownToStart:
                _countdownToStartTimer -= Time.deltaTime;
                if (_countdownToStartTimer < 0f) {
                    _currentState = GameState.GamePlaying;
                    _gamePlayingTimer = GamePlayingTimerMax;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameState.GamePlaying:
                _gamePlayingTimer -= Time.deltaTime;
                if (_gamePlayingTimer < 0f) {
                    _currentState = GameState.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameState.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool IsGamePlaying() {
        return _currentState == GameState.GamePlaying;
    }

    public bool IsCountdownToStartActive() {
        return _currentState == GameState.CountdownToStart;
    }

    public bool IsGameOver() {
        return _currentState == GameState.GameOver;
    }

    public float GetCountdownToStartTimer() {
        return _countdownToStartTimer;
    }

    public float GetGamePlayingTimerNormalized() {
        return 1 - (_gamePlayingTimer / GamePlayingTimerMax);
    }
    public void TogglePauseGame() {
        _isGamePaused = !_isGamePaused;
        if (_isGamePaused) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
        
    }
}
