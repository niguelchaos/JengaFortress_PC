using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public enum GameState
{
    MAIN_MENU,
    SETUP,
    // SET_BOUNDARIES,
    PLACE_FORTRESS,
    PLACE_CORE_BLOCK,
    PLAYING,
    PAUSED,
    GAME_OVER
}

public enum PlayingState
{
    START_TURN,
    IDLE,
    AIMING, // also includes charging up a throw
    // THROWING, 
    END_TURN
}

public enum CurrentPlayer
{
    PLAYER_1 = 1,
    PLAYER_2 = 2
}


public enum WinCondition { HitFloor, LeaveBoundary, Both }


public class GameManager : NetworkBehaviour
{
    // singleton
    public static GameManager Instance;

    // States
    [SerializeField] private GameState gameState;
    [SerializeField] private PlayingState playingState;
    [SerializeField] private CurrentPlayer _currentPlayer;
    [SerializeField] private WinCondition winCondition;

    public static event Action<GameState> OnGameStateChanged;
    public static event Action<PlayingState> OnPlayingStateChanged;
    public static event Action<CurrentPlayer> OnCurrentPlayerChanged;

    [SerializeField] private GameObject _playerPrefab;
    

    
    // awake should contain self setup stuff
    private void Awake()
    {
        Instance = this;
        currentPlayer = CurrentPlayer.PLAYER_1;
        SetGameState(GameState.SETUP);
        SetPlayingState(PlayingState.START_TURN);
    }

    private void Start()
    {
        // set to main menu
        LobbyManager.Instance.MatchHostedEvent += OnMatchHosted;
    }

    public override void OnNetworkSpawn() {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }   

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong playerId) {
        var spawn = Instantiate(_playerPrefab);
        print("Spawning...");
        spawn.GetComponent<NetworkObject>().SpawnWithOwnership(playerId);
    }

    public override void OnDestroy() {
        base.OnDestroy();
        MatchmakingService.LeaveLobby();
        if(NetworkManager.Singleton != null )NetworkManager.Singleton.Shutdown();
    }

    

    


    private void OnMatchHosted()
    {
        SetGameState(GameState.SETUP);
    }

    private void OnGameStarted()
    {
        SetGameState(GameState.PLACE_FORTRESS);
    }



    private void UpdatePlayingState()
    {
        switch(playingState)
        {
            case PlayingState.START_TURN:
                currentPlayer = (currentPlayer == CurrentPlayer.PLAYER_1)
                                    ? CurrentPlayer.PLAYER_2
                                    : CurrentPlayer.PLAYER_1;
                break;
        }
    }


    public void SetGameState(GameState newState)
    {
        gameState = newState;

        // has anybody subscribed to this event? if so broadcast event
        OnGameStateChanged?.Invoke(newState);
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public void SetPlayingState(PlayingState newState)
    {
        playingState = newState;
        UpdatePlayingState();

        OnPlayingStateChanged?.Invoke(newState);
    }

    public PlayingState GetPlayingState()
    {
        return playingState;
    }

    public CurrentPlayer currentPlayer
    {
        get { return _currentPlayer; }
        set { 
            _currentPlayer = value; 
            OnCurrentPlayerChanged?.Invoke(value);
        }
    }

    public WinCondition GetWinCondition()
    {
        return winCondition;
    }

    public void SetWinCondition(WinCondition newWinCondition)
    {
        this.winCondition = newWinCondition; 
    }

    public void StartGame()
    {
        SetGameState(GameState.PLAYING);
    }

    public void goToSetup(){
        SetGameState(GameState.SETUP);
    }
    
    public void ChangePlayer()
    {
        SetPlayingState(PlayingState.START_TURN);
        //currentPlayer = (currentPlayer == CurrentPlayer.PLAYER_1) ? CurrentPlayer.PLAYER_2 : CurrentPlayer.PLAYER_1;
    }







}
