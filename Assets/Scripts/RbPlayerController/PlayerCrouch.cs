using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouch : PlayerClient
{
   private enum State { Idle, Crouching}
   [SerializeField] private State state = State.Idle;

   [SerializeField] private float crouchYScale = 0.75f;
   [SerializeField] private float downForceMultiplier = 5f;

   private void Start()
   {
      Player.startYScale = transform.localScale.y;
      // Player.notNetworkOwnerEvent += OnNotNetworkOwner;
   }
   
   // private void OnNotNetworkOwner()
   // {
   //    Destroy(this);
   // }

   // void OnDestroy()
   // {
   //    Player.notNetworkOwnerEvent -= OnNotNetworkOwner;
   // }

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
         transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
         
         if (Player.isGrounded)
         {
            // Debug.Log("Crouching Force");
            Player.rb.AddForce(Vector3.down * downForceMultiplier, ForceMode.Impulse);
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
         transform.localScale = new Vector3(transform.localScale.x, Player.startYScale, transform.localScale.z);
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
            Player.rb.AddForce(Vector3.down * downForceMultiplier, ForceMode.Impulse);
         }
      }    
   }




}