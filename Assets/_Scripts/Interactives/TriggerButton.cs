using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class TriggerButton : NetworkBehaviour
{

    public UnityEvent onPressed;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(EditorConstants.TAG_PLAYER))
        {
            ulong playerOwner = other.gameObject.transform.parent.GetComponent<NetworkBehaviour>().OwnerClientId;
            if (playerOwner == NetworkManager.Singleton.LocalClientId)
            {
                Pressed(other);
            }
        }
    }

    private void Pressed(Collider other)
    {
        onPressed?.Invoke();
    }

    // [ServerRpc]
    // private void RequestChangePlayerServerRpc()
    // {
    //     // server then calls client to execute
    //     ChangePlayerClientRpc();
    // }

    // // can only be called by server
    // // code within executed on all clients
    // [ClientRpc]
    // private void ChangePlayerClientRpc()
    // {
    //     // from all the clients, will now execute shot
    //     // spawn local version of projectile on everybodys client
       
    //     if (!IsOwner)
    //     {
    //         GameManager.Instance.ChangePlayer();
    //     }
    // }

}
