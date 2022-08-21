using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

// player handling the gun fighting
public class PlayerGunFight : PlayerClient
{
   private enum State { Idle, Windup, Slash}
   [SerializeField] private State state = State.Idle;

   public static Action windupActionEvent;
   public static Action slashActionEvent;

   private void Update()
   {
      switch(state)
      {
         case State.Idle:
            state = UpdateIdleState();
            break;
         case State.Windup:
            state = UpdateWindupState();
            break;
         case State.Slash:
            state = UpdateSlashState();
            break;
      }
   }

   private State UpdateIdleState()
   {
      if (InputManager.Instance.attackInput)
      {
         windupActionEvent?.Invoke();
         return State.Windup;
      }
      else 
      {
         return State.Idle;
      }
   }

   private State UpdateWindupState()
   {
      if (InputManager.Instance.attackInput)
      {
         return State.Windup;
      }
      else 
      {
         slashActionEvent?.Invoke();
         return State.Slash;
      }
   }

   private State UpdateSlashState()
   {
      return State.Idle;
   }
}