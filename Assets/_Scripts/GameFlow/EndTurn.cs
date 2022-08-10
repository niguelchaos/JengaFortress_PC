using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EndTurn : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(EditorConstants.TAG_PLAYER))
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
        // from all the clients, will now execute shot
        // spawn local version of projectile on everybodys client
       
        if (!IsOwner)
        {
            GameManager.Instance.ChangePlayer();
        }
    }

}
