using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


// state machines will be required for more complex games
// probably each enum as a class, or sth like that

// Requires refactor

public enum GameState
{
    SETUP,
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
    // THROWING, 
    END_TURN
}

public enum CurrentPlayer
{
    PLAYER_1 = 1,
    PLAYER_2 = 2
}


public enum WinCondition { HitFloor, LeaveBoundary, Both }


public class GameManager : NetworkSingleton<GameManager>
{
    // States
    [SerializeField] private GameState _gameState;
    [SerializeField] private PlayingState _playingState;
    [SerializeField] private CurrentPlayer _currentPlayer;
    [SerializeField] private WinCondition _winCondition;

    public static event Action<GameState> BeforeGameStateChanged;
    public static event Action<GameState> AfterGameStateChanged;
    // public static event Action<GameState> GameStateChanged;

    public static event Action<PlayingState> PlayingStateChanged;
    public static event Action<CurrentPlayer> CurrentPlayerChanged;

    
    // awake should contain self setup stuff
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        ChangeState(GameState.SETUP);
        currentPlayer = CurrentPlayer.PLAYER_1;
    }

    public void ChangeState(GameState newState)
    {
        if (_gameState == newState)
        {
            print("Same State wtf");
        }

        BeforeGameStateChanged?.Invoke(newState);

        _gameState = newState;
        switch (newState) {
            case GameState.SETUP:
                HandleSetup();
                break;
            case GameState.PLACE_FORTRESS:
                HandlePlaceFortress();
                break;
            case GameState.PLACE_CORE_BLOCK:
                HandlePlaceCoreBlock();
                break;
            case GameState.PLAYING:
                HandlePlaying();
                break;
            default:
            throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        AfterGameStateChanged?.Invoke(newState);
    }

    private void HandleSetup()
    {
        // setup env, cinematics

        // ChangeState(GameState.PLACE_FORTRESS);
    }
    private void HandlePlaceFortress()
    {
        
    }
    private void HandlePlaceCoreBlock()
    {

    }
    private void HandlePlaying()
    {

    }
    

    public override void OnDestroy() {
        base.OnDestroy();

        MatchmakingService.LeaveLobby();
        if(NetworkManager.Singleton != null )NetworkManager.Singleton.Shutdown();
    }


    private void UpdatePlayingState()
    {
        switch(_playingState)
        {
            case PlayingState.START_TURN:
                currentPlayer = (currentPlayer == CurrentPlayer.PLAYER_1)
                                    ? CurrentPlayer.PLAYER_2
                                    : CurrentPlayer.PLAYER_1;
                break;
        }
    }


    // public void SetGameState(GameState newState)
    // {
    //     gameState = newState;

    //     // has anybody subscribed to this event? if so broadcast event
    //     // GameStateChanged?.Invoke(newState);
    // }

    public GameState GetGameState()
    {
        return _gameState;
    }

    public void SetPlayingState(PlayingState newState)
    {
        _playingState = newState;
        UpdatePlayingState();

        PlayingStateChanged?.Invoke(newState);
    }

    public PlayingState GetPlayingState()
    {
        return _playingState;
    }

    public CurrentPlayer currentPlayer
    {
        get { return _currentPlayer; }
        set { 
            _currentPlayer = value; 
            CurrentPlayerChanged?.Invoke(value);
        }
    }

    public WinCondition GetWinCondition()
    {
        return _winCondition;
    }

    public void SetWinCondition(WinCondition newWinCondition)
    {
        this._winCondition = newWinCondition; 
    }
    
    public void ChangePlayer()
    {
        SetPlayingState(PlayingState.START_TURN);
        //currentPlayer = (currentPlayer == CurrentPlayer.PLAYER_1) ? CurrentPlayer.PLAYER_2 : CurrentPlayer.PLAYER_1;
    }







}
