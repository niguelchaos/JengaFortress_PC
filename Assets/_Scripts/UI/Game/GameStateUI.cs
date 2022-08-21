using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStateUI : MonoBehaviour
{
    [SerializeField] private TMP_Text currentGameState; 
    [SerializeField] private TMP_Text currentPlayer; 


    private void Start()
    {
        GameManager.BeforeGameStateChanged += UpdateGameState;
        GameManager.AfterGameStateChanged += UpdateGameState;
        GameManager.CurrentPlayerChanged += UpdateCurrentPlayer;

        UpdateCurrentPlayer(GameManager.Instance.currentPlayer);
        UpdateGameState(GameManager.Instance.GetGameState());
    }

    private void UpdateCurrentPlayer(CurrentPlayer player)
    {
        currentPlayer.text = player.ToString() + "'s turn";
    }
    private void UpdateGameState(GameState gameState)
    {
        currentGameState.text = gameState.ToString();
    }

    private void OnDestroy()
    {
        GameManager.BeforeGameStateChanged -= UpdateGameState;
        GameManager.AfterGameStateChanged -= UpdateGameState;
        GameManager.CurrentPlayerChanged -= UpdateCurrentPlayer;
    }

}