using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class DebugUI : MonoBehaviour
{
    public TMP_Text currentGameStateText;
    public TMP_Text currentPlayerText;
    public TMP_Text lobbyStateText;

    [SerializeField] private GameObject gameStateCube;
    private Renderer cubeRenderer;

    [SerializeField] private GameObject menuCanvas; 

    private void Start()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
        GameManager.OnPlayingStateChanged += OnPlayingStateChanged;
        LobbyManager.LobbyUpdateStateEvent += OnLobbyUpdateState;
        // LobbyManager.Instance.MatchHostedEvent += OnMatchFound;

        gameStateCube = GameObject.Find("GameStateCube");
        if (gameStateCube != null)
        {
            cubeRenderer = gameStateCube.GetComponent<Renderer>();
        }

    }

    private void OnGameStateChanged(GameState newGameState)
    {
        if (currentGameStateText != null) {
            currentGameStateText.text = newGameState.ToString();
        }

        UpdateGameStateUI(newGameState);
    }

    private void OnPlayingStateChanged(PlayingState newPlayingState)
    {
        if (currentPlayerText != null)
        {
            currentPlayerText.text = GameManager.Instance.currentPlayer.ToString();
        }
    }

    private void OnLobbyUpdateState(LobbyManager.LobbyUpdateState newState)
    {
        lobbyStateText.text = "";
        lobbyStateText.text = newState.ToString();
    }

    private void UpdateGameStateUI(GameState newGameState)
    {
        if (cubeRenderer != null)
        {
            switch(newGameState)
            {
                case GameState.SETUP:
                    cubeRenderer.material.color = Color.white;
                    break;
                case GameState.PLAYING:
                    //Call SetColor using the shader property name "_Color" and setting the color to red
                    if(GameManager.Instance.currentPlayer is CurrentPlayer.PLAYER_1)
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

    private void OnGUI() {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
            if (GUILayout.Button("Local Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Local Server")) NetworkManager.Singleton.StartServer();
            if (GUILayout.Button("Local Client")) NetworkManager.Singleton.StartClient();
        }

        GUILayout.EndArea();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
        GameManager.OnPlayingStateChanged -= OnPlayingStateChanged;
        LobbyManager.LobbyUpdateStateEvent -= OnLobbyUpdateState;
    }
}