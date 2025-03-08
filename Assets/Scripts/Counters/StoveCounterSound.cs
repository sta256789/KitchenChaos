using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour {

    [SerializeField] private StoveCounter stoveCounter;

    private AudioSource _audioSource;
    private float _warningSoundTimer;
    private bool _playWarningSound;

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    private void Update() {
        if (_playWarningSound) {
            _warningSoundTimer -= Time.deltaTime;
            if (_warningSoundTimer <= 0f) {
                const float warningSoundTimerMax = .2f;
                _warningSoundTimer = warningSoundTimerMax;
                
                SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
            }
        }
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
        const float burnShowProgressAmount = .5f;
        _playWarningSound = stoveCounter.IsFried() && e.ProgressNormalized >= burnShowProgressAmount;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e) {
        bool playSound = e.CurrentState == StoveCounter.State.Frying || e.CurrentState == StoveCounter.State.Fried;
        if (playSound) {
            _audioSource.Play();
        }
        else {
            _audioSource.Pause();
        }
    }
}
