using System;
using Unity.Netcode;
using UnityEngine;

public class CustomClientNetworkTransform : NetworkBehaviour
{
    private NetworkVariable<Vector2> _currentPlayerPosition = new(Vector2.zero);
    private NetworkVariable<sbyte> _currentPlayerOrientation = new(1);

    private Rigidbody2D _rigidbody;
    private float _originalTransformXScale;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        _currentPlayerPosition.OnValueChanged += OnPositionChanged;
        _currentPlayerOrientation.OnValueChanged += OnOrientationChanged;
        _originalTransformXScale = transform.localScale.x;
    }

    private void OnOrientationChanged(sbyte previousOrientation, sbyte currentOrientation)
    {
        if (IsOwner) return;
        UpdateTransformOrientation(currentOrientation);
    }

    private void OnPositionChanged(Vector2 previousPosition, Vector2 currentPosition)
    {
        if (IsOwner) return;
        // UpdatedSimulatedPlayerPosition();
    }

    private void Update()
    {
        if (IsOwner) return;

        // render entity position
        var nextPos = new Vector3(_currentPlayerPosition.Value.x, _currentPlayerPosition.Value.y, transform.position.z);
        // transform.position = Vector3.Lerp(transform.position, nextPos, _tickDeltaTime / _tickRate);
        transform.position = nextPos;

        // render entity orientation
        var scale = transform.localScale;
        scale.x = _originalTransformXScale * _currentPlayerOrientation.Value;
        transform.localScale = scale;
    }

    private void UpdateTransformOrientation(sbyte orientation)
    {
        var transformScale = transform.localScale;
        var scale = new Vector3(
            _originalTransformXScale * orientation,
            transformScale.y,
            transformScale.z
        );
        transform.localScale = scale;
    }

    public void MoveOwnerPlayer(Vector2 movementDirection, float moveSpeed)
    {
        _rigidbody.velocity = movementDirection * moveSpeed * Time.fixedDeltaTime;
        UpdateOwnerPositionServerRpc(_rigidbody.position);
    }

    public void UpdateOwnerOrientation(sbyte orientation)
    {
        UpdateTransformOrientation(orientation);
        UpdateOwnerOrientationServerRpc(orientation);
    }

    [ServerRpc]
    private void UpdateOwnerPositionServerRpc(Vector2 position)
    {
        _currentPlayerPosition.Value = position;
    }

    [ServerRpc]
    private void UpdateOwnerOrientationServerRpc(sbyte orientation)
    {
        _currentPlayerOrientation.Value = orientation;
    }
}