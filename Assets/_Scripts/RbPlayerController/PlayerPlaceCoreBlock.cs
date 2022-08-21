using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerPlaceCoreBlock: PlayerClient 
{
    public enum State { Idle, Place, Move }
    [SerializeField] private State state = State.Idle;

    public static Action StartPlaceCoreBlock;
    public static Action PlacedCoreBlock;

    private void Start()
    {
        GameManager.BeforeGameStateChanged += OnGameStateChanged;
    }
    
    private void OnDestroy()
    {
        GameManager.BeforeGameStateChanged -= OnGameStateChanged;
    }


    private void Update()
    {
        switch(state)
        {
            case State.Idle:
                break;
            case State.Place:
                state = UpdatePlaceState();
                break;
            case State.Move:
                state = UpdateMoveState();
                break;
        }
    }

    private State UpdateIdleState()
    {
        return State.Idle;
    }

    private State UpdatePlaceState()
    {
        // confirmed selection
        if (InputManager.Instance.firePressed)
        {
            PlaceCoreBlock();
            return State.Idle;
        }

        return State.Place;
    }

    private void PlaceCoreBlock()
    {
        PlacedCoreBlock?.Invoke();
    }
   
    private State UpdateMoveState()
    {
        return State.Idle;
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.PLACE_CORE_BLOCK)
        {
            state = State.Place;
            StartPlaceCoreBlock?.Invoke();
        }
    }

}