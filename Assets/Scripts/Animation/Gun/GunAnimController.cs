using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

// player handling the gun
public class GunAnimController : NetworkBehaviour
{
   private enum State { Idle, PointBack, Fire, Reloading}
   [SerializeField] private State state = State.Idle;

   private Animator animator;
   private bool subEventsOnStart = true;

   private void Awake()
   {
      animator = GetComponent<Animator>();
   }

   private void Start()
   { 
      if (subEventsOnStart)
      {
         PlayerGunFight.windupActionEvent += Windup;
         PlayerGunFight.slashActionEvent += Slash;
         print("anim subbed");
      }
   }

   private void Update()
   {

   }

   private void Windup()
   {
      PointBack();
      RequestWindupServerRpc();
   }

   private void Slash()
   {
      Idle();
      RequestSlashServerRpc();
   }

   private void PointBack()
   {
      if (animator != null)
      {
         animator.SetBool("isPointingBack", true);
      }
   }
   private void Idle()
   {
      if (animator != null)
      {
         animator.SetBool("isPointingBack", false);
      }
   }

   // private void OnDestroy()
   // {
   //    UnsubEvents();
   // }

   public void UnsubEvents()
   {
      subEventsOnStart = false;
      PlayerGunFight.windupActionEvent -= Windup;
      PlayerGunFight.slashActionEvent -= Slash;
      print("anim unsubbed");
   }

   [ServerRpc]
   private void RequestWindupServerRpc()
   {
      // server then calls client to execute
      WindupClientRpc();
   }
   
   [ServerRpc]
   private void RequestSlashServerRpc()
   {
      SlashClientRpc();
   }

   // can only be called by server
   // code within executed on all clients
   [ClientRpc]
   private void WindupClientRpc()
   {
      // from all the clients, will now execute shot
      // spawn local version of projectile on everybodys client
      
      if (!IsOwner)
      {
         PointBack();
      }
   }
   [ClientRpc]
   private void SlashClientRpc()
   {
      if (!IsOwner)
      {
         Idle();
      }
   }



}