using UnityEngine;
using Unity.Netcode;

// player handling the gun
public class PlayerAnimController : NetworkBehaviour
{
   [SerializeField] private PlayerMovement playerMovement; // there must be better way to obtain moveVel - violates layering
   private Animator animator;
   private bool subEventsOnStart = true;

   [SerializeField] private Transform cameraYaw;

   [SerializeField] private float animBlendSpeed = 0.08f;
   [SerializeField] private Vector3 currentVel;
   private int xVelHash, yVelHash;
   

   private void Awake()
   {
      animator = GetComponent<Animator>();

      xVelHash = Animator.StringToHash("X_Velocity");
      yVelHash = Animator.StringToHash("Y_Velocity");
   }

   private void Start()
   { 
      if (subEventsOnStart)
      {
         // PlayerGunFight.windupActionEvent += Windup;
         // PlayerGunFight.slashActionEvent += Slash;


         print("player anim subbed");
      }
   }

   private void FixedUpdate()
   {
      currentVel.x = Mathf.Lerp(currentVel.x, playerMovement.moveVel.x, animBlendSpeed);
      currentVel.z = Mathf.Lerp(currentVel.z, playerMovement.moveVel.z, animBlendSpeed);

      animator.SetFloat(xVelHash, currentVel.x);
      animator.SetFloat(yVelHash, currentVel.z);
   }

   private void LateUpdate()
   {
      transform.rotation = cameraYaw.rotation;
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

   public override void OnDestroy()
   {
      base.OnDestroy();
      
      UnsubEvents();
   }

   public void UnsubEvents()
   {
      subEventsOnStart = false;
      // PlayerGunFight.windupActionEvent -= Windup;
      // PlayerGunFight.slashActionEvent -= Slash;
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