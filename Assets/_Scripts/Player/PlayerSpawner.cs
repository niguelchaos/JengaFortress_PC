using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _spawnLocation;

    private void Start()
    {
        // set to main menu
        // NetworkManager.Singleton.OnClientConnectedCallback += OnLobbyEnteredClient;
        // NetworkManager.Singleton.OnServerStarted += OnLobbyEnteredServer;
        // print("spawner subbed");

        
    }

    public override void OnNetworkSpawn() 
    {
        if (SceneManager.GetActiveScene().name == "FindTheCore" ||
            SceneManager.GetActiveScene().name == "LobbyRoom")
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }   

    public override void OnDestroy() {
        base.OnDestroy();

        // GetComponent<NetworkObject>().Despawn();
        // print("Despawning PlayerSpawner");

        // NetworkManager.Singleton.OnClientConnectedCallback -= OnLobbyEnteredClient;
        // NetworkManager.Singleton.OnServerStarted -= OnLobbyEnteredServer;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong playerId) {
        GameObject spawn;
        if (_spawnLocation != null)
        {
            spawn = Instantiate(_playerPrefab, _spawnLocation.transform.position, Quaternion.identity);
        }
        else {
            spawn = Instantiate(_playerPrefab);
        }
        print("Spawning...");
        spawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerId, true);
    }

    private void OnLobbyEnteredClient(ulong id)
    {
        if (!IsServer)
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }
    private void OnLobbyEnteredServer()
    {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }

}