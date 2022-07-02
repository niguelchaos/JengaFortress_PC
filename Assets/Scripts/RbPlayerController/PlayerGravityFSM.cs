using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravityFSM : PlayerClient
{
   private enum State { Normal, Wallrun }
   [SerializeField] private State state = State.Normal;

   [SerializeField] private float wallRunGravity = 2f;
   [SerializeField] private float defaultGravityScale = 5;
   
   private void Start()
   {
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
         case State.Normal:
            state = UpdateState();
            break;
         case State.Wallrun:
            state = UpdateState();
            break;
      }
      
   }

   private void FixedUpdate()
   {
      ControlGravity();
   }

   private State UpdateState()
   {
      if (Player.isWallRunning)
      {
         return State.Wallrun;
      }
      else {
         return State.Normal;
      }
   }
   

   private void ControlGravity()
   {
      switch(state)
      {
         case State.Normal:
            Player.rb.useGravity = true;
            Player.rb.AddForce(Physics.gravity * defaultGravityScale, ForceMode.Acceleration);
            break;
         case State.Wallrun:
            Player.rb.useGravity = false;
            Player.rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);
            break;
      }
   }

}