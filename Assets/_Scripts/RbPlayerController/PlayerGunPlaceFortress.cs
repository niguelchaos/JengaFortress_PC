using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum State
{
    IDLE,
    PLACE, 
    SELECT,
    MOVE
}

public class PlayerGunPlaceFortress: PlayerClient 
{
    [SerializeField] private State state = State.PLACE;

    public static Action placeFortressActionEvent;

    private void Start()
    {
    }

    private void Update()
    {
    //   ReceiveInput();
      switch(state)
      {
         case State.PLACE:
            state = UpdatePlaceState();
            break;
         case State.MOVE:
            state = UpdateMoveState();
            break;
      }
   }

    private State UpdatePlaceState()
    {
        if (InputManager.Instance.fireInput)
        {
            return State.PLACE;
        }
        else 
        {
            return State.IDLE;
        }
    }
   
   private State UpdateMoveState()
    {
        return State.IDLE;
    }

    // public void ReceiveInput()
    // {
    //     if (InputManager.Instance.fireInput == false)
    //     {
    //         Player.hasFired = false;
    //     }
    // }
}