using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

// player handling the gun firing mechanism
public class PlayerGunstick : PlayerClient
{
   private enum State { Idle, Fire, Reloading}
   [SerializeField] private State state = State.Idle;
   [SerializeField] private PlayerGunstickData GunstickData;


   public static Action fireActionEvent;
   public static Action reloadActionEvent;

   private void Start()
   { 
      Gun.propelActionEvent += Propel;
   }

   private void OnDestroy()
   {
      Gun.propelActionEvent -= Propel;
   }

   private void Update()
   {
      // ReceiveInput();
      switch(state)
      {
         case State.Idle:
            state = UpdateIdleState();
            break;
         case State.Fire:
            state = UpdateFireState();
            break;
         case State.Reloading:
            state = UpdateReloadState();
            break;
      }
   }
   
   private void FixedUpdate()
   {
      switch(state)
      {
         case State.Idle:
            break;
         case State.Fire:
            Fire();
            break;
         case State.Reloading:
            break;
      }
   }

   private State UpdateIdleState()
   {
      if (InputManager.Instance.fireInput)
      {
         return State.Fire;
      }
      else if (InputManager.Instance.reloadInput)
      {
         return State.Reloading;
      }
      else 
      {
         return State.Idle;
      }
   }
   
   private State UpdateFireState()
   {
      return State.Idle;
   }

   private State UpdateReloadState()
   {
      reloadActionEvent?.Invoke();
      return State.Idle;
   }

   private void Fire()
   {
      fireActionEvent?.Invoke();  
   }

   private void Propel(GameObject muzzle)
   {
      Vector3 propelDir = GetDistance(muzzle);
      Player.rb.AddRelativeForce(propelDir * GunstickData.propelForce, ForceMode.Impulse);
   }

   private Vector3 GetDistance(GameObject muzzle)
   {
      Vector3 playerPos = Player.transform.position;
      Vector3 dir = (this.transform.position - muzzle.transform.position).normalized;
      Debug.DrawLine (playerPos, playerPos + dir * 10, Color.red, Mathf.Infinity);
      return dir;
   }

   // public void ReceiveInput()
   // {
   //    if (InputManager.Instance.fireInput == false)
   //    {
   //       Player.hasFired = false;
   //    }
   // }
}