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



public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    private string _lobbyId;

    private RelayManager.RelayHostData _hostData;
    private RelayManager.RelayJoinData _joinData;

    // Setup events
    
    // Notify state update
    public UnityAction<string> LobbyUpdateStateEvent;
    // Notify Match found
    public UnityAction MatchFoundEvent;
    public UnityAction MatchHostedEvent;

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            return;
        }

        Destroy(this);
    }

    async void Start()
    {
        // init unity services
        await UnityServices.InitializeAsync();

        // setup events listeners
        SetupEvents();

        // Unity Login
        // After logging in, can use services
        await SignInAnonymouslyAsync();

        // Subscribe to NetworkManager events
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;

    }

    #region Network events

    private void ClientConnected(ulong id)
    {
        // Player with id connected to our session
        
        Debug.Log("Connected player with id: " + id);
        
        LobbyUpdateStateEvent?.Invoke("Player found!");
        MatchFoundEvent?.Invoke();
    }

    #endregion

    #region UnityLogin

    void SetupEvents() {
        AuthenticationService.Instance.SignedIn += () => {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
        };

        AuthenticationService.Instance.SignInFailed += (err) => {
            Debug.LogError(err);
        };

        AuthenticationService.Instance.SignedOut += () => {
            Debug.Log("Player signed out.");
        };
    }
    
    async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");
        }
        catch (Exception ex)
        {
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    #endregion

    #region Lobby

    public async void FindMatch()
    {
        Debug.Log("Looking for a lobby...");

        LobbyUpdateStateEvent?.Invoke("Looking for a match...");
        
        try
        {
            // Looking for a lobby - search for open lobbies (Quick Matchmaking)
            
            // Add options to the matchmaking (mode, rank, etc..)
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            // Quick-join a random lobby
            Lobby lobby = await Lobbies.Instance.QuickJoinLobbyAsync(options);
                
            Debug.Log("Joined lobby: " + lobby.Id);
            Debug.Log("Lobby Players: " + lobby.Players.Count);
            
            // Retrieve the Relay code previously set in the create match
            string joinCode = lobby.Data["joinCode"].Value;
                
            Debug.Log("Received code: " + joinCode);
            
            JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
                
            // Create Object
            _joinData = new RelayManager.RelayJoinData
            {
                Key = allocation.Key,
                Port = (ushort) allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                HostConnectionData = allocation.HostConnectionData,
                IPv4Address = allocation.RelayServer.IpV4
            };

            // Set transport data
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                _joinData.IPv4Address, 
                _joinData.Port, 
                _joinData.AllocationIDBytes, 
                _joinData.Key, 
                _joinData.ConnectionData, 
                _joinData.HostConnectionData);
                
            // Finally start the client
            NetworkManager.Singleton.StartClient();
            
            // Trigger events
            LobbyUpdateStateEvent?.Invoke("Match found!");
            MatchFoundEvent?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            // If we don't find any lobby, let's create a new one
            Debug.Log("Cannot find a lobby: " + e);
            CreateMatch();
        }
    }

    private async void CreateMatch()
    {
         Debug.Log("Creating a new lobby...");
        
        LobbyUpdateStateEvent?.Invoke("Creating a new match...");
        
        // External connections
        int maxConnections = 1;
        
        try
        {
            // Create RELAY object
            Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
            _hostData = new RelayManager.RelayHostData
            {
                Key = allocation.Key,
                Port = (ushort) allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                IPv4Address = allocation.RelayServer.IpV4
            };
            
            // Retrieve JoinCode, allowing lobby players to join host via relay
            _hostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            string lobbyName = "game_lobby";
            int maxPlayers = 2;
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;
            
            // Put the JoinCode in the lobby data, visible by every member
            // options.data allows sharing data between lobby players and externals 
            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    "joinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: _hostData.JoinCode)
                },
            };

            // Create the lobby
            var lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            
            // Save Lobby ID for later uses
            _lobbyId = lobby.Id;
            
            Debug.Log("Created lobby: " + lobby.Id);
            
            // Heartbeat the lobby every 15 seconds.
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));
            
            // Now that RELAY and LOBBY are set...
            
            // Set Transports data
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                _hostData.IPv4Address, 
                _hostData.Port, 
                _hostData.AllocationIDBytes, 
                _hostData.Key, 
                _hostData.ConnectionData);
                
            // Finally start host
            NetworkManager.Singleton.StartHost();
            MatchHostedEvent?.Invoke();
            
            LobbyUpdateStateEvent?.Invoke("Waiting for players...");
        }
        catch (LobbyServiceException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    // Need to ping lobby or else Unity will shut em down
    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            Debug.Log("Lobby Heartbeat");
            yield return delay;
        }
    }

    private void OnDestroy()
    {
        // We need to delete the lobby when we're not using it
        Lobbies.Instance.DeleteLobbyAsync(_lobbyId);
    }

    #endregion
}