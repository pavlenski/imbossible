using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class TempPlayerCtrl : NetworkBehaviour
{
    // [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private PlayerInputCtrl _playerInput;
    [SerializeField] private float _moveSpeed;

    private void Awake()
    {
        // _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInputCtrl>();
    }

    private void FixedUpdate()
    {
        var force = _playerInput.MovementDirection() * _moveSpeed * Time.fixedDeltaTime;
        transform.position += new Vector3(force.x, force.y, transform.position.z);
    }
}