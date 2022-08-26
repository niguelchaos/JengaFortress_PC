using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : PlayerClient
{
   private enum State { Normal, Wallrun, InAir, Fly }
   [SerializeField] private State state = State.Normal;

   [SerializeField] private PlayerGravityData GravityData;
   [SerializeField] private float currentAirGravity = 5f;

   private void Update()
   {
      state = UpdateState();

      switch(state) {
         case State.Fly:
         Player.rb.useGravity = false;
         break;

         default:
         Player.rb.useGravity = true;
         break;
      }
   }

   private void FixedUpdate()
   {
      if (state != State.Fly)
      {
         ControlGravity();
      }
   }

   private State UpdateState()
   {
      if (Player.flyMode)
      {
         return State.Fly;
      }
      else 
      {
         if (Player.isWallRunning)
         {
            return State.Wallrun;
         }
         else if (Player.isFalling)
         {
            return State.InAir;
         }
         else {
            return State.Normal;
         }
      }
   }
   

   private void ControlGravity()
   {
      switch(state)
      {
         case State.Normal:
            Player.rb.useGravity = true;
            ResetInAirGravity();
            Player.rb.AddForce(Physics.gravity * GravityData.defaultGravityScale, ForceMode.Acceleration);
            break;
         case State.Wallrun:
            Player.rb.useGravity = false;
            ResetInAirGravity();
            Player.rb.AddForce(Vector3.down * GravityData.wallRunGravity, ForceMode.Force);
            break;
         case State.Fly:
            ResetInAirGravity();
            Player.rb.useGravity = false;
            break;
         case State.InAir:
            Player.rb.useGravity = true;
            currentAirGravity = currentAirGravity + (GravityData.inAirGravMultiplier * Time.deltaTime);
            Player.rb.AddForce(Physics.gravity * currentAirGravity, ForceMode.Acceleration);
            break;
      }
   }

   private void ResetInAirGravity()
   {
      currentAirGravity = GravityData.defaultGravityScale;
   }

}