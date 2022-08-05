using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;


using Object = UnityEngine.Object;

public static class MatchmakingService {
    private const int HeartbeatInterval = 15;
    private const int LobbyRefreshRate = 2; // Rate limits at 2
    private const int MinPlayers = 1;
    private const int MaxPlayers = 10;

    private static UnityTransport _transport;

    private static Lobby _currentLobby;
    private static CancellationTokenSource _heartbeatSource, _updateLobbySource;

    private static UnityTransport Transport {
        get => _transport != null ? _transport : _transport = Object.FindObjectOfType<UnityTransport>();
        set => _transport = value;
    }

    public static event Action<Lobby> CurrentLobbyRefreshed;

    public static void ResetStatics() {
        if (Transport != null) {
            Transport.Shutdown();
            Transport = null;
        }

        _currentLobby = null;
    }

    // Obviously you'd want to add customization to the query, but this
    // will suffice for this simple demo
    public static async Task<List<Lobby>> GatherLobbies() {
        var options = new QueryLobbiesOptions {
            Count = 15,

            Filters = new List<QueryFilter> {
                new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                new(QueryFilter.FieldOptions.IsLocked, "0", QueryFilter.OpOptions.EQ)
            }
        };

        var allLobbies = await LobbyService.Instance.QueryLobbiesAsync(options);
        return allLobbies.Results;
    }

    public static async Task CreateLobbyWithAllocation(LobbyData data) {

        if (data.MaxPlayers > MaxPlayers) data.MaxPlayers = MaxPlayers;
        else if (data.MaxPlayers < MinPlayers) data.MaxPlayers = MinPlayers;
        
        // Create a relay allocation and generate a join code to share with the lobby
        Allocation a = await RelayService.Instance.CreateAllocationAsync(data.MaxPlayers);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

        // Create a lobby, adding the relay join code to the lobby data
        CreateLobbyOptions options = new CreateLobbyOptions {
            Data = new Dictionary<string, DataObject> 
            {
                { LobbyConstants.JoinKey, new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                // { LobbyConstants.GameTypeKey, new DataObject(DataObject.VisibilityOptions.Public, data.Type.ToString(), DataObject.IndexOptions.N1) }, 
                // {
                //     LobbyConstants.DifficultyKey,
                //     new DataObject(DataObject.VisibilityOptions.Public, data.Difficulty.ToString(), DataObject.IndexOptions.N2)
                // }
            }
        };

        _currentLobby = await Lobbies.Instance.CreateLobbyAsync(data.Name, data.MaxPlayers, options);
        Debug.Log("Created Joincode: " + _currentLobby.Data[LobbyConstants.JoinKey].Value);

        Transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

        Heartbeat();
        PeriodicallyRefreshLobby();
    }


    public static async Task JoinLobbyWithAllocation(string lobbyId) {
        _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
        var a = await RelayService.Instance.JoinAllocationAsync(_currentLobby.Data[LobbyConstants.JoinKey].Value);
        Debug.Log("Joincode: " + _currentLobby.Data[LobbyConstants.JoinKey].Value);


        Transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

        PeriodicallyRefreshLobby();
    }

    public static async Task JoinLobby(Lobby lobby) {
        _currentLobby = lobby;
        Debug.Log("Current Lobby: " + _currentLobby.Id);

        var a = await RelayService.Instance.JoinAllocationAsync(_currentLobby.Data[LobbyConstants.JoinKey].Value);

        Transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

        PeriodicallyRefreshLobby();
    }

    public static async Task QuickJoinLobby()
    {
        // Add options to the matchmaking (mode, rank, etc..)
        QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();
        // Quick-join a random lobby
        Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
        
        string joinCode = lobby.Data[LobbyConstants.JoinKey].Value;
        Debug.Log("Joincode: " + joinCode);

        await JoinLobby(lobby);
        Debug.Log("Quick Joining Lobby");
    }

    public static async Task QuickCreateLobby()
    {
        await CreateLobbyWithAllocation(LobbyConstants.DefaultLobbyData);
        Debug.Log("Quick Lobby Created");
    }

    public static async Task LockLobby() {
        try {
            await Lobbies.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions { IsLocked = true });
        }
        catch (Exception e) {
            Debug.Log($"Failed closing lobby: {e}");
        }
    }

    public static async Task LeaveLobby() {
        _heartbeatSource?.Cancel();
        _updateLobbySource?.Cancel();

        if (_currentLobby != null)
        {
            try {
                if (_currentLobby.HostId == AuthService.PlayerId) await LobbyService.Instance.DeleteLobbyAsync(_currentLobby.Id);
                else await Lobbies.Instance.RemovePlayerAsync(_currentLobby.Id, AuthService.PlayerId);
                _currentLobby = null;
            }
            catch (Exception e) {
                Debug.Log(e);
            }
        }
    }

    private static async void Heartbeat() {
        _heartbeatSource = new CancellationTokenSource();
        while (!_heartbeatSource.IsCancellationRequested && _currentLobby != null) {
            await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
            await Task.Delay(HeartbeatInterval * 1000);
        }
    }

    private static async void PeriodicallyRefreshLobby() {
        _updateLobbySource = new CancellationTokenSource();
        await Task.Delay(LobbyRefreshRate * 1000);
        while (!_updateLobbySource.IsCancellationRequested && _currentLobby != null) {
            _currentLobby = await Lobbies.Instance.GetLobbyAsync(_currentLobby.Id);
            CurrentLobbyRefreshed?.Invoke(_currentLobby);
            await Task.Delay(LobbyRefreshRate * 1000);
        }
    }

    
}