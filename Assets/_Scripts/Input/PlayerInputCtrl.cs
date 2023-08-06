using Unity.Netcode;
using UnityEngine;

public class PlayerInputCtrl : NetworkBehaviour
{
    private PlayerInput _playerInput;
    private Vector2 _movementInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable() => _playerInput.gameplay.Enable();
    private void OnDisable() => _playerInput.gameplay.Disable();

    private void Update()
    {
        if (!IsOwner) return;
        _movementInput = _playerInput.gameplay.movement.ReadValue<Vector2>();
    }

    public Vector2 MovementDirection() => _movementInput;
}