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
        if (other.CompareTag(EditorConstants.PLAYER_TAG))
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

}
