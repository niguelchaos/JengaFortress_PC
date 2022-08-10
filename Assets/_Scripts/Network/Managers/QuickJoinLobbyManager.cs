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

public class QuickJoinLobbyManager : NetworkBehaviour
{
    public static QuickJoinLobbyManager Instance;

    private Lobby connectedLobby;
    private UnityTransport transport; 

    private string _lobbyId;
    private string joinCode;
    [SerializeField] private int maxConnections = 4;
    [SerializeField] private string joinCodeKey = "joinCode";
    private int lobbyHeartbeatFrequency = 15;


    private void Awake()
    {

        if (Instance is null)
        {
            Instance = this;
            return;
        }
        else {
            Destroy(this);
        }
    }

    private void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    }

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

    #region Quick Join

    public async void CreateOrJoinLobby()
    {
        // await Authenticate();
        
        // first try quick join, then if no work create lobby
        connectedLobby = await QuickJoinLobby() ?? await CreateLobby();
        if (connectedLobby != null) {}
    }

    public async void CreateOrJoinLobbyTest()
    {
        // await Authenticate();
        await AuthService.Login();
        
        // first try quick join, then if no work create lobby
        connectedLobby = await QuickJoinLobby() ?? await CreateLobby();
        if (connectedLobby != null) {}
    }

    private static async Task Authenticate()
    {
        var options = new InitializationOptions();
        
        #if UNITY_EDITOR
            // for parrelsync
            // differentiate clients
            options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
            print("using parrelsync");
        #endif

        try
        {
            // init unity services
            await UnityServices.InitializeAsync(options);

            // setup events listeners
            QuickJoinLobbyManager.Instance.SetupEvents();

            // Unity Login
            // After logging in, can use services
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            var playerID = AuthenticationService.Instance.PlayerId;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private async Task<Lobby> QuickJoinLobby()
    {
        // Looking for a lobby - search for open lobbies (Quick Matchmaking)
        Debug.Log("Looking for a lobby...");
        // LobbyManager.LobbyUpdateStateEvent?.Invoke(LobbyManager.LobbyUpdateState.FINDING_LOBBY);

        try
        {    
            // Add options to the matchmaking (mode, rank, etc..)
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();
            // Quick-join a random lobby
            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);

            Debug.Log("Joined lobby: " + lobby.Id);
            Debug.Log("Lobby Players: " + lobby.Players.Count);

            // Retrieve the Relay code previously set in the create match
            string joinCode = lobby.Data["joinCode"].Value;

            Debug.Log("Received code: " + joinCode);

            // join with the found code
            JoinAllocation alloc = await Relay.Instance.JoinAllocationAsync(joinCode);

            // set transport data
            transport.SetClientRelayData(alloc.RelayServer.IpV4, (ushort)alloc.RelayServer.Port, alloc.AllocationIdBytes, alloc.Key, alloc.ConnectionData, alloc.HostConnectionData);

            // Finally start the client
            NetworkManager.Singleton.StartClient();

            // Trigger events
            // LobbyManager.LobbyUpdateStateEvent?.Invoke(LobbyManager.LobbyUpdateState.FOUND_LOBBY);
            // GameStarted?.Invoke();
            // LobbyManager.Instance.MatchFoundEvent?.Invoke();
            
            return lobby;
        }
        catch (LobbyServiceException e)
        {
            // If we don't find any lobby, let's create a new one
            Debug.Log("Cannot find a lobby for Quick Join: " + e);
            // create lobby
            return null;
        }
    }

    private async Task<Lobby> CreateLobby()
    {
        Debug.Log("Creating a new lobby...");
        // LobbyManager.LobbyUpdateStateEvent?.Invoke(LobbyManager.LobbyUpdateState.CREATING_LOBBY);
                
        try
        {
            // Create RELAY object/allocation and join code 
            Allocation alloc = await Relay.Instance.CreateAllocationAsync(maxConnections);
            
            // Retrieve JoinCode, allowing lobby players to join host via relay
            string joinCode = await Relay.Instance.GetJoinCodeAsync(alloc.AllocationId);
            Debug.Log("joincode: " + joinCode);
            
            string lobbyName = "game_lobby";
            int maxPlayers = 2;

            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;
            
            // Put the JoinCode in the lobby data, visible by every member
            // options.data allows sharing data between lobby players and externals 
            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    joinCodeKey, new DataObject(
                        visibility: DataObject.VisibilityOptions.Public, // was VisibilityOptions.member
                        value: joinCode)
                },
            };

            // Create the lobby
            var lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            
            // Save Lobby ID for later uses
            _lobbyId = lobby.Id;
            
            Debug.Log("Created lobby: " + lobby.Id);
            
            // Heartbeat the lobby every 15 seconds to keep room alive.
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, lobbyHeartbeatFrequency));
            
            // Now that RELAY and LOBBY are set...
            
            // Set Transports data
            transport.SetHostRelayData(alloc.RelayServer.IpV4, (ushort)alloc.RelayServer.Port, alloc.AllocationIdBytes, alloc.Key, alloc.ConnectionData);
                
            // Finally start host
            NetworkManager.Singleton.StartHost();
            
            // LobbyManager.Instance.MatchHostedEvent?.Invoke();
            // GameStarted?.Invoke();
            // LobbyManager.LobbyUpdateStateEvent?.Invoke(LobbyManager.LobbyUpdateState.WAITING_FOR_PLAYERS);

            return lobby;
        }
        catch (LobbyServiceException e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    // Need to ping lobby or else Unity will shut em down
    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            // Debug.Log("Lobby Heartbeat");
            yield return delay;
        }
    }

    #endregion

   
    #region Basic Join Code Connection

    public async void CreateGame()
    {
        // initialize connection to relay by creating allocation on relay service
        Allocation alloc = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

        // transport through relay system
        transport.SetHostRelayData(alloc.RelayServer.IpV4, (ushort)alloc.RelayServer.Port, alloc.AllocationIdBytes, alloc.Key, alloc.ConnectionData);
        
        NetworkManager.Singleton.StartHost();
    }

    public async void JoinGame()
    {
        // initialize connection to relay by creating allocation on relay service
        JoinAllocation alloc = await RelayService.Instance.JoinAllocationAsync(joinCode);
        // joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

        // transport through relay system
        transport.SetClientRelayData(alloc.RelayServer.IpV4, (ushort)alloc.RelayServer.Port, alloc.AllocationIdBytes, alloc.Key, alloc.ConnectionData, alloc.HostConnectionData);
        
        NetworkManager.Singleton.StartClient();
    }
    #endregion
    

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        try
        {
            StopAllCoroutines();

            if (connectedLobby != null)
            {
                // We need to delete the lobby when we're not using it
                // if (connectedLobby.HostId == playerId) Lobbies.Instance.DeleteLobbyAsync(_lobbyId);
                
                // prevent phantom lobbies 
                // else { Lobbies.Instance.RemovePlayerAsync(connectedLobby.id, playerid)}
                LobbyService.Instance.DeleteLobbyAsync(_lobbyId);
            }
        }
        catch (Exception e) {
            Debug.Log($"Error shutting down lobby: {e}");
        }
    }
}