using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnFlashingUI : MonoBehaviour {

    private const string IS_FLASHING = "IsFlashing";
    
    [SerializeField] private StoveCounter stoveCounter;

    private Animator _animator;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void Start() {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
        
        _animator.SetBool(IS_FLASHING, false);
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
        const float burnShowProgressAmount = .5f;
        bool show = stoveCounter.IsFried() && e.ProgressNormalized >= burnShowProgressAmount;

        _animator.SetBool(IS_FLASHING, show);
    }
}
