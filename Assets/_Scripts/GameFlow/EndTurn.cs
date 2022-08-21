using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EndTurn : NetworkBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(EditorConstants.PLAYER_TAG))
        {
            GameManager.Instance.ChangePlayer();
            print("turn ended");
        }
    }

    [ServerRpc]
    private void RequestChangePlayerServerRpc()
    {
        // server then calls client to execute
        ChangePlayerClientRpc();
    }

    // can only be called by server
    // code within executed on all clients
    [ClientRpc]
    private void ChangePlayerClientRpc()
    {
        // from all the clients, will now execute
        // update local version 
       
        if (!IsOwner)
        {
            GameManager.Instance.ChangePlayer();
        }
    }

}
