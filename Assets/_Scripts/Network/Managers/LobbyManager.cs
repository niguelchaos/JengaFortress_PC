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
using UnityEngine.Events;
using UnityEngine.SceneManagement;


// Lobby code taken and adapted from Tarodevs tut

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private MainLobbyScreen _mainLobbyScreen;
    [SerializeField] private CreateLobbyScreen _createScreen;
    [SerializeField] private RoomScreen _roomScreen;
    
    // Notify state update
    // public static UnityAction<LobbyUpdateState> LobbyUpdateStateEvent;
    public enum LobbyUpdateState {
        FINDING_LOBBY, FOUND_LOBBY, CREATING_LOBBY, WAITING_FOR_PLAYERS, FOUND_PLAYER
    }
    
    // Notify Match found
    public static event Action LobbyEntered;

    private void Start()
    {
        // Subscribe to NetworkManager events
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;

        CreateLobbyScreen.LobbyCreated += CreateLobby;
        LobbyRoomPanel.LobbySelected += OnLobbySelected;
        RoomScreen.LobbyLeft += OnLobbyLeft;
        RoomScreen.StartPressed += OnGameStart;

        NetworkObject.DestroyWithScene = true;
    }

    #region Enter Lobby

    private async void OnLobbySelected(Lobby lobby) {
        // using (new Load("Joining Lobby...")) {
            try {
                print("Joining Lobby");
                await MatchmakingService.JoinLobbyWithAllocation(lobby.Id);

                _mainLobbyScreen.gameObject.SetActive(false);
                _roomScreen.gameObject.SetActive(true);

                NetworkManager.Singleton.StartClient();
                print("Client started");

                
                // Trigger events
                LobbyEntered?.Invoke();
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

                _mainLobbyScreen.gameObject.SetActive(false);
                _createScreen.gameObject.SetActive(false);
                _roomScreen.gameObject.SetActive(true);

                // Starting the host immediately will keep the relay server alive
                NetworkManager.Singleton.StartHost();
                print("host started");

                // Trigger events
                LobbyEntered?.Invoke();
                
            }
            catch (Exception e) {
                Debug.LogError(e);
                throw e;
                // CanvasUtilities.Instance.ShowError("Failed creating lobby");
            }
        // }
    }

    #endregion

    #region Quick Join

    public async void QuickJoinLobby()
    {
        // Looking for a lobby - search for open lobbies (Quick Matchmaking)
        Debug.Log("Looking for a lobby...");

        try
        {    
            await MatchmakingService.QuickJoinLobby();

            _mainLobbyScreen.gameObject.SetActive(false);
            _roomScreen.gameObject.SetActive(true);

            NetworkManager.Singleton.StartClient();
            
            // Trigger events
            LobbyEntered?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            // If we don't find any lobby, let's create a new one
            Debug.Log("Cannot find a lobby for Quick Join: " + e);
            
            _mainLobbyScreen.gameObject.SetActive(false);
            _createScreen.gameObject.SetActive(true);
            // Debug.Log("Quick Lobby Created");
        }
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

        if (IsServer) {
            return;
        }
        

        if (!_playersInLobby.ContainsKey(clientId)) _playersInLobby.Add(clientId, isReady);
        else _playersInLobby[clientId] = isReady;

        // print(_playersInLobby[clientId] + " is " + isReady);
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
            _roomScreen.gameObject.SetActive(false);
            _mainLobbyScreen.gameObject.SetActive(true);
            OnLobbyLeft();
        }
    }

    [ClientRpc]
    private void RemovePlayerClientRpc(ulong clientId) {
        if (IsServer) {
            return;
        }

        if (_playersInLobby.ContainsKey(clientId)) _playersInLobby.Remove(clientId);
        UpdateInterface();
    }

    public void OnReadyClicked() {
        // print("Sending player " + NetworkManager.Singleton.LocalClientId);
        SetReadyServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetReadyServerRpc(ulong playerId) {
        
        _playersInLobby[playerId] = !_playersInLobby[playerId];
        // print("Serverrpc > player " + playerId + "  is ready: " + _playersInLobby[playerId]);

        PropagateToClients();
        UpdateInterface();
    }

    private void UpdateInterface() {
        LobbyPlayersUpdated?.Invoke(_playersInLobby);
    }

    private async void OnLobbyLeft() {
        // using (new Load("Leaving Lobby...")) {
            _playersInLobby.Clear();

            // maybe when going back to main menu shutdown
            NetworkManager.Singleton.Shutdown();
            await MatchmakingService.LeaveLobby();
            print("Leaving Lobby");

            _mainLobbyScreen.gameObject.SetActive(true);
            _roomScreen.gameObject.SetActive(false);

        // }
    }

    private async void OnGameStart() {
        // using (new Load("Starting the game...")) {
            await MatchmakingService.LockLobby();
            print("Locking Lobby, starting game");

            GoToFindTheCoreScene();
            // GameStartClientRpc();
        // }
    }

    private void GoToFindTheCoreScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Scenes/FindTheCore", LoadSceneMode.Single);   
    }

    [ClientRpc]
    private void GameStartClientRpc() {
        if (IsServer || IsHost) return;

        print("clients changing scene");
        GoToFindTheCoreScene();
    }


    
    public override void OnDestroy() {
        base.OnDestroy();

        // StopAllCoroutines();

        CreateLobbyScreen.LobbyCreated -= CreateLobby;
        LobbyRoomPanel.LobbySelected -= OnLobbySelected;
        RoomScreen.LobbyLeft -= OnLobbyLeft;
        
        // We only care about this during lobby
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }      
        print("Destroying LobbyManager: " + SceneManager.GetActiveScene().name);

    }

    public void OnApplicationQuit()
    {
        print("Quitting app");
        if (IsServer)
        {
            OnLobbyLeft();
        }
    }
    

    #endregion

    #region Network events

    private void ClientConnected(ulong id)
    {
        // Player with id connected to our session
        Debug.Log("Connected player with id: " + id);
        
        // LobbyUpdateStateEvent?.Invoke(LobbyUpdateState.FOUND_PLAYER);
        LobbyEntered?.Invoke();
    }

    #endregion

    


    
}