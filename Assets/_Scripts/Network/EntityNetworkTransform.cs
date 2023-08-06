using Unity.Netcode;
using UnityEngine;

public class EntityNetworkTransform : NetworkBehaviour
{
    private NetworkVariable<Vector2> _currentPosition = new(Vector2.zero);
    private NetworkVariable<sbyte> _currentOrientation = new(1);

    private Rigidbody2D _rigidbody;
    private float _originalTransformXScale;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _originalTransformXScale = transform.localScale.x;
    }

    // This logic is run locally on the client
    private void Update()
    {
        if (!IsClient || IsServer) return;

        // position & orient entity
        SetEntityLocalPosition();
        SetEntityLocalOrientation();
    }

    private void SetEntityLocalPosition()
    {
        var nextPos = new Vector3(_currentPosition.Value.x, _currentPosition.Value.y, transform.position.z);
        transform.position = nextPos;
    }

    private void SetEntityLocalOrientation()
    {
        var scale = transform.localScale;
        scale.x = _originalTransformXScale * _currentOrientation.Value;
        transform.localScale = scale;
    }

    // This could trigger a ClientRPC to inform Clients rendering the entity of its position
    public void MoveEntity(Vector2 moveDir, float speed)
    {
        if (!IsServer)
        {
            Debug.LogError("[MoveEntity] should only be called from the server!");
            return;
        }

        // Debug.Log($"what, ms {speed} - mdir {moveDir}");
        _rigidbody.velocity = moveDir * speed * Time.fixedDeltaTime;
        _currentPosition.Value = _rigidbody.position;
    }

    public void StopEntity()
    {
        if (!IsServer)
        {
            Debug.LogError("[StopEntity] should only be called from the server!");
            return;
        }

        _rigidbody.velocity = Vector2.zero;
        _currentPosition.Value = Vector2.zero;
    }

    public void OrientEntity(sbyte orientation)
    {
        _currentOrientation.Value = orientation;
        // This is to update the orientation on the server
        SetEntityLocalOrientation();
    }
}