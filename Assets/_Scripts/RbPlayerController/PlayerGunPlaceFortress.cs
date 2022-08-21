using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class PlayerGunPlaceFortress: PlayerClient 
{
    public enum State
    {   IDLE, PLACE, SELECT, MOVE }

    [SerializeField] private State state = State.PLACE;

    public static Action placeFortressActionEvent;

    private void Start()
    {
    }

    private void Update()
    {
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
        if (InputManager.Instance.firePressed)
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

}