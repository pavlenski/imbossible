using System;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerCtrl : NetworkBehaviour
{
    private const string IS_MOVING = "is_moving";

    [SerializeField] private PlayerInputCtrl _playerInput;
    [SerializeField] private GameObject _fireBoltPrefab;

    private Animator _animator;
    private ServerAuthTransform _networkTransform;

    // positive for right, negative for left
    private sbyte _lastOrientation = 1;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInputCtrl>();
        _animator = GetComponentInChildren<Animator>();
        _networkTransform = GetComponent<ServerAuthTransform>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;

        SpawnProjectileServerRpc(Vector2.right);
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
        _networkTransform.MoveOwnerPlayer(movementDirection);
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

    [ServerRpc]
    private void SpawnProjectileServerRpc(Vector2 projectileDirection)
    {
        var go = Instantiate(_fireBoltPrefab, transform.position, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
        go.GetComponent<ProjectileCtrl>().Init(projectileDirection);
    }
}