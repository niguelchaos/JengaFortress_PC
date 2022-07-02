using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraMover : NetworkBehaviour
{
    [SerializeField] Transform camTransform;
    private GameObject cam;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>().gameObject;
        if (!IsOwner)
        {
            Destroy(cam);
        }
        // Player.notNetworkOwnerEvent += OnNotNetworkOwner;
    }

    // private void OnNotNetworkOwner()
    // {
    //     // Destroy(cam);
    //     Destroy(this);
    // }

    // void OnDestroy()
    // {
    //     Player.notNetworkOwnerEvent -= OnNotNetworkOwner;
    // }

    private void Update()
    {
        transform.position = camTransform.position;
        // transform.rotation = camTransform.rotation;
    }
    
}