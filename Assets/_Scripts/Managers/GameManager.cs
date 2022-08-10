using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


// state machines will be required for more complex games
// probably each enum as a class, or sth like that

public enum GameState
{
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
    [SerializeField] private GameState gameState;
    [SerializeField] private PlayingState playingState;
    [SerializeField] private CurrentPlayer _currentPlayer;
    [SerializeField] private WinCondition winCondition;

    public static event Action<GameState> OnBeforeGameStateChanged;
    public static event Action<GameState> OnAfterGameStateChanged;

    public static event Action<GameState> OnGameStateChanged;
    public static event Action<PlayingState> OnPlayingStateChanged;
    public static event Action<CurrentPlayer> OnCurrentPlayerChanged;

    
    // awake should contain self setup stuff
    protected override void Awake()
    {
        base.Awake();

        currentPlayer = CurrentPlayer.PLAYER_1;
        SetGameState(GameState.SETUP);
        SetPlayingState(PlayingState.START_TURN);
    }
    private void Start()
    {
        ChangeState(GameState.SETUP);
    }

    public void ChangeState(GameState newState)
    {
        if (gameState == newState)
        {
            print("Same State wtf");
        }

        OnBeforeGameStateChanged?.Invoke(newState);

        gameState = newState;
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

        OnAfterGameStateChanged?.Invoke(newState);
    }

    private void HandleSetup()
    {
        // setup env, cinematics

        ChangeState(GameState.PLACE_FORTRESS);
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
    
    public void ChangePlayer()
    {
        SetPlayingState(PlayingState.START_TURN);
        //currentPlayer = (currentPlayer == CurrentPlayer.PLAYER_1) ? CurrentPlayer.PLAYER_2 : CurrentPlayer.PLAYER_1;
    }







}
