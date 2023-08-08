using System;
using Unity.Netcode;
using UnityEngine;

public class ServerAuthTransform : NetworkBehaviour
{
    private NetworkVariable<Vector2> _currentPlayerPosition = new(Vector2.zero);
    private NetworkVariable<sbyte> _currentPlayerOrientation = new(1);

    [SerializeField] private float _moveSpeed;

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
        UpdateTransformOrientation(currentOrientation);
    }

    private void OnPositionChanged(Vector2 previousPosition, Vector2 currentPosition)
    {
        var nextPos = new Vector3(currentPosition.x, currentPosition.y, transform.position.z);
        transform.position = nextPos;
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

    public void MoveOwnerPlayer(Vector2 movementDirection)
    {
        UpdateOwnerPositionServerRpc(movementDirection);
    }

    public void UpdateOwnerOrientation(sbyte orientation)
    {
        UpdateOwnerOrientationServerRpc(orientation);
    }

    [ServerRpc]
    private void UpdateOwnerPositionServerRpc(Vector2 movementDirection)
    {
        _rigidbody.velocity = movementDirection * _moveSpeed * Time.fixedDeltaTime;
        _currentPlayerPosition.Value = _rigidbody.position;
    }

    [ServerRpc]
    private void UpdateOwnerOrientationServerRpc(sbyte orientation)
    {
        UpdateTransformOrientation(orientation);
        _currentPlayerOrientation.Value = orientation;
    }
}