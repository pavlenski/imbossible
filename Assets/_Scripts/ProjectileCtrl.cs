using Unity.Netcode;
using UnityEngine;

public class ProjectileCtrl : NetworkBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 moveDir)
    {
        _rigidbody.velocity = moveDir * _speed * Time.fixedDeltaTime;
    }

    // TODO rework this after spawning the projectile server-side
    private void OnTriggerEnter2D(Collider2D _)
    {
        Debug.Log($"{gameObject.name} HIT {_.gameObject.name}");
        // Temporary destroying on client side 
    }
}