using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCtrl : MonoBehaviour
{
    private const string GAMEPLAY_SCENE = "Gameplay";
    private const string LOCAL_SERVER_IP = "192.168.0.11";
    private const ushort PORT = 7777;

    private NetworkManager _networkManager;

    private void Start()
    {
        _networkManager = NetworkManager.Singleton;
    }

    public void StartServer()
    {
        _networkManager.GetComponent<UnityTransport>().SetConnectionData(
            default,
            (ushort) 7777,
            "0.0.0.0"
        );

        _networkManager.StartServer();
        _networkManager.SceneManager.LoadScene(GAMEPLAY_SCENE, LoadSceneMode.Single);
    }

    public void StartClient()
    {
        _networkManager.GetComponent<UnityTransport>().SetConnectionData(
            LOCAL_SERVER_IP,
            PORT
        );

        _networkManager.StartClient();
    }
}