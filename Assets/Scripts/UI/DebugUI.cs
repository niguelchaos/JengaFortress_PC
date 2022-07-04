using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{
    public TMP_Text currentGameStateText;
    public TMP_Text currentPlayerText;

    [SerializeField] private GameObject gameStateCube;
    private Renderer cubeRenderer;

    private void Start()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
        GameManager.OnPlayingStateChanged += OnPlayingStateChanged;

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
}