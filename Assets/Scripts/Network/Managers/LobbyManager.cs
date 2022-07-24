using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using ParrelSync;
#endif

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;
    
    // Notify state update
    public UnityAction<LobbyUpdateState> LobbyUpdateStateEvent;
    public enum LobbyUpdateState {
        FINDING_LOBBY, FOUND_LOBBY, CREATING_LOBBY, WAITING_FOR_PLAYERS, FOUND_PLAYER
    }
    
    // Notify Match found
    public event Action MatchFoundEvent;
    public event Action MatchHostedEvent;
    

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            return;
        }

        Destroy(this);
    }

    private void Start()
    {
        // Subscribe to NetworkManager events
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
        QuickJoinLobbyManager.Instance.GameStarted += OnGameStart;

    }

    #region Enter Lobby

    private async void OnLobbySelected(Lobby lobby) {
        // using (new Load("Joining Lobby...")) {
            try {
                await MatchmakingService.JoinLobbyWithAllocation(lobby.Id);

                // _mainLobbyScreen.gameObject.SetActive(false);
                // _roomScreen.gameObject.SetActive(true);

                NetworkManager.Singleton.StartClient();
                
                // Trigger events
                LobbyUpdateStateEvent?.Invoke(LobbyUpdateState.FOUND_LOBBY);
                MatchFoundEvent?.Invoke();
            }
            catch (Exception e) {
                Debug.LogError(e);
                // CanvasUtilities.Instance.ShowError("Failed joining lobby");
            }
        // }
    }

    #endregion

    #region Create Lobby

    private async void CreateLobby(LobbyData data) {
        // using (new Load("Creating Lobby...")) {
            try {
                await MatchmakingService.CreateLobbyWithAllocation(data);

                // _createScreen.gameObject.SetActive(false);
                // _roomScreen.gameObject.SetActive(true);

                // Starting the host immediately will keep the relay server alive
                NetworkManager.Singleton.StartHost();

                // Trigger events
                MatchHostedEvent?.Invoke();
                LobbyUpdateStateEvent?.Invoke(LobbyUpdateState.WAITING_FOR_PLAYERS);
            }
            catch (Exception e) {
                Debug.LogError(e);
                // CanvasUtilities.Instance.ShowError("Failed creating lobby");
            }
        // }
    }

    #endregion

    #region Room

    private readonly Dictionary<ulong, bool> _playersInLobby = new();
    public static event Action<Dictionary<ulong, bool>> LobbyPlayersUpdated;
    private float _nextLobbyUpdate;

    public override void OnNetworkSpawn() {
        if (IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            _playersInLobby.Add(NetworkManager.Singleton.LocalClientId, false);
            UpdateInterface();
        }

        // Client uses this in case host destroys the lobby
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

 
    }

     private void OnClientConnectedCallback(ulong playerId) {
        if (!IsServer) return;

        // Add locally
        if (!_playersInLobby.ContainsKey(playerId)) _playersInLobby.Add(playerId, false);

        PropagateToClients();

        UpdateInterface();
    }

    private void PropagateToClients() {
        foreach (var player in _playersInLobby) UpdatePlayerClientRpc(player.Key, player.Value);
    }

    [ClientRpc]
    private void UpdatePlayerClientRpc(ulong clientId, bool isReady) {
        if (IsServer) return;

        if (!_playersInLobby.ContainsKey(clientId)) _playersInLobby.Add(clientId, isReady);
        else _playersInLobby[clientId] = isReady;
        UpdateInterface();
    }

    private void OnClientDisconnectCallback(ulong playerId) {
        if (IsServer) {
            // Handle locally
            if (_playersInLobby.ContainsKey(playerId)) _playersInLobby.Remove(playerId);

            // Propagate all clients
            RemovePlayerClientRpc(playerId);

            UpdateInterface();
        }
        else {
            // This happens when the host disconnects the lobby
            // _roomScreen.gameObject.SetActive(false);
            // _mainLobbyScreen.gameObject.SetActive(true);
            OnLobbyLeft();
        }
    }

    [ClientRpc]
    private void RemovePlayerClientRpc(ulong clientId) {
        if (IsServer) return;

        if (_playersInLobby.ContainsKey(clientId)) _playersInLobby.Remove(clientId);
        UpdateInterface();
    }

    public void OnReadyClicked() {
        SetReadyServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetReadyServerRpc(ulong playerId) {
        _playersInLobby[playerId] = true;
        PropagateToClients();
        UpdateInterface();
    }

    private void UpdateInterface() {
        LobbyPlayersUpdated?.Invoke(_playersInLobby);
    }

    private async void OnLobbyLeft() {
        // using (new Load("Leaving Lobby...")) {
            _playersInLobby.Clear();
            NetworkManager.Singleton.Shutdown();
            await MatchmakingService.LeaveLobby();
        // }
    }
    private async void OnGameStart() {
        // using (new Load("Starting the game...")) {
            await MatchmakingService.LockLobby();
            print("Locking Lobby, starting game");

            if (SceneManager.GetActiveScene().name == "Lobby")
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Scenes/GameTest", LoadSceneMode.Single);
            }
        // }
    }
    
    public override void OnDestroy() {
     
        base.OnDestroy();

        StopAllCoroutines();

        // CreateLobbyScreen.LobbyCreated -= CreateLobby;
        // LobbyRoomPanel.LobbySelected -= OnLobbySelected;
        // RoomScreen.LobbyLeft -= OnLobbyLeft;
        QuickJoinLobbyManager.Instance.GameStarted -= OnGameStart;

        
        // We only care about this during lobby
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }
      
    }
    

    #endregion

    #region Network events

    private void ClientConnected(ulong id)
    {
        // Player with id connected to our session
        Debug.Log("Connected player with id: " + id);
        
        LobbyUpdateStateEvent?.Invoke(LobbyUpdateState.FOUND_PLAYER);
        MatchFoundEvent?.Invoke();
    }

    #endregion

    


    
}