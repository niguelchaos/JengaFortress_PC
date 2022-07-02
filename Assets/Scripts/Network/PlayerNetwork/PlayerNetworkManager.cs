using Unity;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerNetworkManager : NetworkBehaviour
{
    [SerializeField] private Player player;

    [Header("Network")]
    public static Action<ulong> notNetworkOwnerEvent;


    private void Start()
    {
        if (IsOwner)
        {
            player.SetupPlayerInput();
        }
        else {
            print("Spawned player Not owner");
        }
    }

    public override void OnNetworkSpawn()
    {
        player = transform.Find("PlayerBody").gameObject.GetComponent<Player>();

        // code running on all computers, but only want to control own character
        if (!IsOwner) 
        {
            // tell everybody else
            notNetworkOwnerEvent?.Invoke(NetworkManager.Singleton.LocalClientId);

            player.DisablePlayerInput();

            // disable gun controls
            GameObject weaponHolder = this.transform.Find("CameraHolder/WeaponHolder").gameObject;
            if (weaponHolder != null)
            {
                weaponHolder.GetComponent<WeaponSway>().enabled = false;
            }
            else { print("shiet"); }
            
            GameObject gunStick = this.transform.Find("CameraHolder/WeaponHolder/Gun/GunStick").gameObject;
            if (gunStick != null)
            {
                gunStick.GetComponent<Gun>().UnsubEvents();
                gunStick.GetComponent<GunAnimController>().UnsubEvents();
            }
            else { print("shiet"); }


            print("Disabled controls for " + NetworkManager.LocalClientId);
        }
    }
}