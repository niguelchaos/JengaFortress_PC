using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : PlayerClient
{
   private enum State { Idle, Jumping, Walljump}
   [SerializeField] private State state = State.Idle;
   
   [SerializeField] private PlayerJumpData JumpData;
   
   private void Update()
   {
      switch(state)
      {
         case State.Idle:
            state = UpdateIdleState();
            break;
         case State.Jumping:
            state = UpdateJumpingState();
            break;
         case State.Walljump:
            state = UpdateWalljumpState();
            break;
      }
   }
   private void FixedUpdate()
   {
      switch(state)
      {
         case State.Idle:
            break;
         case State.Jumping:
            Jump();
            break;
         case State.Walljump:
            WallJump();
            break;
      }
   }

   private State UpdateIdleState()
   {
      if (InputManager.Instance.jumpPressed && Player.isGrounded)
      {
         return State.Jumping;
      }
      else if (InputManager.Instance.jumpPressed && Player.isWallRunning)
      {
         return State.Walljump;
      }
      else {
         return State.Idle;
      }
   }
   
   private State UpdateJumpingState()
   {
      return State.Idle;
   }
   private State UpdateWalljumpState()
   {
      return State.Idle;
   }


   private void Jump()
   {
      // print("-- jumping");
      Player.rb.velocity = new Vector3(Player.rb.velocity.x, 0, Player.rb.velocity.z);
      Player.rb.AddForce(transform.up * JumpData.jumpForce, ForceMode.Impulse);
      
   }

   private void WallJump()
   {

      // print("-- walljumping");
      if (Player.isWallLeft)
      {
            Vector3 wallJumpDir = (transform.up + Player.leftWallHit.normal).normalized;
            Player.rb.velocity = new Vector3(Player.rb.velocity.x, 0, Player.rb.velocity.z);
            Player.rb.AddForce(wallJumpDir * JumpData.wallJumpForce * 100, ForceMode.Force);
      }
      else if (Player.isWallRight)
      {
            Vector3 wallJumpDir = (transform.up + Player.rightWallHit.normal).normalized;
            Player.rb.velocity = new Vector3(Player.rb.velocity.x, 0, Player.rb.velocity.z);
            Player.rb.AddForce(wallJumpDir * JumpData.wallJumpForce * 100, ForceMode.Force);
      }
   }
}