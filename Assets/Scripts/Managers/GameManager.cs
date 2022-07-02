using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameState
{
    MAIN_MENU,
    SETUP,
    SET_BOUNDARIES,
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
    THROWING, 
    END_TURN
}

public enum CurrentPlayer
{
    PLAYER_1 = 1,
    PLAYER_2 = 2
}

public enum CoreLoopMode
{
    FIRE_BLOCK,
    MOVE_BLOCK
}

public enum WinCondition { HitFloor, LeaveBoundary, Both }


public class GameManager : MonoBehaviour
{
    // singleton
    public static GameManager Instance;

    // States
    [SerializeField] private GameState gameState;
    [SerializeField] private PlayingState playingState;
    [SerializeField] private CurrentPlayer _currentPlayer;
    [SerializeField] private WinCondition winCondition;
    [SerializeField] private CoreLoopMode _coreLoopMode;

    public static event Action<GameState> OnGameStateChanged;
    public static event Action<PlayingState> OnPlayingStateChanged;
    public static event Action<CurrentPlayer> OnCurrentPlayerChanged;
    public static event Action<CoreLoopMode> OnCoreLoopModeChanged;

    // 
    [SerializeField] private GameObject gameStateCube;
    private Renderer cubeRenderer;

    public TMP_Text currentGameStateText;
    
    // awake should contain self setup stuff
    private void Awake()
    {
        Instance = this;
        currentPlayer = CurrentPlayer.PLAYER_1;
        SetGameState(GameState.MAIN_MENU);
        SetPlayingState(PlayingState.START_TURN);
    }

    private void Start()
    {
        // set to main menu
        gameStateCube = GameObject.Find("GameStateCube");
        if (gameStateCube != null)
        {
            cubeRenderer = gameStateCube.GetComponent<Renderer>();
        }
    }

    //private void Update() {}

    private void UpdateGameState()
    {
        if (cubeRenderer != null)
        {
            switch(gameState)
            {
                case GameState.MAIN_MENU:
                    cubeRenderer.material.color = Color.white;
                    break;
                case GameState.PLAYING:
                    //Call SetColor using the shader property name "_Color" and setting the color to red
                    if(currentPlayer is CurrentPlayer.PLAYER_1)
                        cubeRenderer.material.color = Color.yellow;
                    else
                        cubeRenderer.material.color = Color.blue;
                    break;
                case GameState.GAME_OVER:
                    cubeRenderer.material.color = Color.red;
                    break;
            }
        }
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
            /*case PlayingState.THROWING | PlayingState.AIMING:
                break;
            case PlayingState.END_TURN:
                break;*/
        }
    }


    public void SetGameState(GameState newState)
    {
        gameState = newState;
        UpdateGameState();
        currentGameStateText.text = GetGameState().ToString();

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
        //SetCurrentPlayer((CurrentPlayer.PLAYER_1 & CurrentPlayer.PLAYER_2) ^ currentPlayer);
    }

    public CoreLoopMode coreLoopMode
    {
        get { return _coreLoopMode; }
        set {
            _coreLoopMode = value;
            OnCoreLoopModeChanged?.Invoke(value);
        }
    }

    public void SetCoreLoopFire()
    {
        _coreLoopMode = CoreLoopMode.FIRE_BLOCK;
    }
    public void SetCoreLoopMove()
    {
        _coreLoopMode = CoreLoopMode.MOVE_BLOCK;
    }


}
