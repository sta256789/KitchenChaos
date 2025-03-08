using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {

    public static Player Instance { get; private set; }

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter SelectedCounter;
    }
    
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool _isWalking;
    private Vector3 _lastInteractDirection;
    private BaseCounter _selectedCounter;
    private KitchenObject _kitchenObject;

    private void Awake() {
        if (Instance != null) {
            Debug.Log("There is more than one Player instance.");
        }
        Instance = this;
    }

    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;
        
        if (_selectedCounter != null) {
            _selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;
        
        if (_selectedCounter != null) {
            _selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }
    
    public bool IsWalking()
    {
        return _isWalking;
    }

    private void HandleMovement() {
        var inputVector = gameInput.GetMovementVectorNormalized();
        var moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        var playerPosition = transform.position;

        var moveDistance = moveSpeed * Time.deltaTime;
        const float playerRadius = .7f;
        const float playerHeight = 2f;
        const float diagonalToleranceMin = -0.5f;
        const float diagonalToleranceMax = -0.5f;
        var canMove = !Physics.CapsuleCast(playerPosition, playerPosition + Vector3.up * playerHeight, playerRadius,
            moveDirection, moveDistance);

        if (!canMove) {
            // Cannot move towards moveDirection
            
            // Attempt only X movement
            var moveDirectionX = new Vector3(moveDirection.x, 0, 0).normalized;
            canMove = (moveDirection.x < diagonalToleranceMin || moveDirection.x > diagonalToleranceMax) && !Physics.CapsuleCast(playerPosition, playerPosition + Vector3.up * playerHeight, playerRadius,
                moveDirectionX, moveDistance);

            if (canMove) {
                // Can move only on the X
                moveDirection = moveDirectionX;
            }
            else {
                // Can not move on the X
                
                // Attempt only Z movement
                var moveDirectionZ = new Vector3(moveDirection.x, 0, 0).normalized;
                canMove = (moveDirection.z < diagonalToleranceMin || moveDirection.z > diagonalToleranceMax) && !Physics.CapsuleCast(playerPosition, playerPosition + Vector3.up * playerHeight, playerRadius,
                    moveDirectionZ, moveDistance);

                if (canMove) {
                    // Can move only on the X
                    moveDirection = moveDirectionZ;
                }
            }
        }

        if (canMove) {
            playerPosition += moveDistance * moveDirection;
            transform.position = playerPosition;
        }

        _isWalking = moveDirection != Vector3.zero;
        
        const float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);
    }

    private void HandleInteractions() {
        var inputVector = gameInput.GetMovementVectorNormalized();
        var moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        var playerPosition = transform.position;

        if (moveDirection != Vector3.zero) {
            _lastInteractDirection = moveDirection;
        }
        
        const float interactDistance = 2f;
        if (Physics.Raycast(playerPosition, _lastInteractDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                // Has ClearCounter
                if (baseCounter != _selectedCounter) {
                    SetSelectedCounter(baseCounter);
                }
            }
            else {
                SetSelectedCounter(null);
            }
        }
        else {
            SetSelectedCounter(null);
        }
    }

    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this._selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {SelectedCounter = _selectedCounter});
    }


    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this._kitchenObject = kitchenObject;

        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject() {
        return _kitchenObject;
    }

    public void ClearKitchenObject() {
        _kitchenObject = null;
    }

    public bool HasKitchenObject() {
        return _kitchenObject != null;
    }
}
