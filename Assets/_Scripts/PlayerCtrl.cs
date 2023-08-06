using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerCtrl : NetworkBehaviour
{
    private const string IS_MOVING = "is_moving";

    [SerializeField] private PlayerInputCtrl _playerInput;
    [SerializeField] private float _moveSpeed = 5f;

    private Animator _animator;
    private CustomClientNetworkTransform _networkTransform;

    // positive for right, negative for left
    private sbyte _lastOrientation = 1;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInputCtrl>();
        _animator = GetComponentInChildren<Animator>();
        _networkTransform = GetComponent<CustomClientNetworkTransform>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        var movementDirection = _playerInput.MovementDirection();
        HandleMovement(movementDirection);
        HandleAnimation(movementDirection);
        HandleOrientation(movementDirection);
    }

    private void HandleMovement(Vector2 movementDirection)
    {
        _networkTransform.MoveOwnerPlayer(movementDirection, _moveSpeed);
    }

    private void HandleAnimation(Vector2 movementDirection)
    {
        _animator.SetBool(IS_MOVING, movementDirection != Vector2.zero);
    }

    private void HandleOrientation(Vector2 movementDirection)
    {
        if (movementDirection.x < 0)
        {
            _lastOrientation = -1;
            _networkTransform.UpdateOwnerOrientation(_lastOrientation);
        }
        else if (movementDirection.x > 0)
        {
            _lastOrientation = 1;
            _networkTransform.UpdateOwnerOrientation(_lastOrientation);
        }
    }
}