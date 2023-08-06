using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private float _maxSpawnX, _maxSpawnY;

    [SerializeField] private GameObject _orcPrefab;

    private void Update()
    {
        if (!IsServer) return;
        if (!Input.GetKeyDown(KeyCode.O)) return;

        var go = Instantiate(_orcPrefab, Vector3.zero, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
    }
}
