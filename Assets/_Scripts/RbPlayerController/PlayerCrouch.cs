using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouch : PlayerClient
{
   private enum State { Idle, Crouching}
   [SerializeField] private State state = State.Idle;
   [SerializeField] private PlayerCrouchData CrouchData;

   private void Start()
   {
      Player.Data.startYScale = transform.localScale.y;
   }

   private void Update()
   {
      switch(state)
      {
         case State.Idle:
            state = UpdateIdleState();
            break;
         case State.Crouching:
            state = UpdateCrouchState();
            break;
      }
   }
   private void FixedUpdate()
   {
      switch(state)
      {
         case State.Idle:
            // FixedUpdateIdleState();
            break;
         case State.Crouching:
            break;
      }
   }

   private State UpdateIdleState()
   {
      if (InputManager.Instance.isCrouching)
      {
         transform.localScale = new Vector3(transform.localScale.x, CrouchData.crouchYScale, transform.localScale.z);
         
         if (Player.isGrounded)
         {
            // Debug.Log("Crouching Force");
            Player.rb.AddForce(Vector3.down * CrouchData.downForceMultiplier, ForceMode.Impulse);
         }
         return State.Crouching;
      }
      else {
         return State.Idle;
      }
   }
   
   private State UpdateCrouchState()
   {
      if (InputManager.Instance.isCrouching)
      {
         return State.Crouching;
      }
      else 
      {
         transform.localScale = new Vector3(transform.localScale.x, Player.Data.startYScale, transform.localScale.z);
         return State.Idle;
      }
   }

   private void FixedUpdateIdleState()
   {
      if (InputManager.Instance.isCrouching)
      {
         if (Player.isGrounded)
         {
            Debug.Log("Crouching Force");
            Player.rb.AddForce(Vector3.down * CrouchData.downForceMultiplier, ForceMode.Impulse);
         }
      }    
   }




}