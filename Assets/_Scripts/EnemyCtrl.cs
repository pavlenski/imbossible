using Unity.Netcode;
using UnityEngine;

public class EnemyCtrl : NetworkBehaviour
{
    private const string IS_MOVING = "is_moving";

    [SerializeField] private float _speed;
    [SerializeField] private Animator _animator;
    [SerializeField] private EntityNetworkTransform _networkTransform;

    private Transform _playerTransform;
    private Vector2 _movementDirection;
    private sbyte _lastOrientation = 1;

    private void FixedUpdate()
    {
        if (!IsServer) return;

        FindPlayer();
        MoveTowardsPlayer();
        HandleAnimations();
        HandleOrientation();
    }

    private void HandleAnimations()
    {
        _animator.SetBool(IS_MOVING, _movementDirection != Vector2.zero);
    }

    private void FindPlayer()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go == null) return;

        _playerTransform = go.transform;
    }

    private void HandleOrientation()
    {
        if (_movementDirection.x < 0)
        {
            _lastOrientation = -1;
        }
        else if (_movementDirection.x > 0)
        {
            _lastOrientation = 1;
        }

        _networkTransform.OrientEntity(_lastOrientation);
    }

    private void MoveTowardsPlayer()
    {
        if (_playerTransform == null)
        {
            Debug.Log($"[{gameObject.name}]: No player found, not moving");
            _movementDirection = Vector2.zero;
            _networkTransform.StopEntity();
            return;
        }

        _movementDirection = (_playerTransform.position - transform.position).normalized;
        _networkTransform.MoveEntity(_movementDirection, _speed);
    }
}