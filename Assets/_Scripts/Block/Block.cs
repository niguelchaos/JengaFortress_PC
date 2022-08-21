using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class Block : NetworkBehaviour
{
    public Rigidbody rb {get; private set;}
    private NetworkRigidbody netRb;
    private NetworkObject netObj;
    [SerializeField] private float magnitude;
    [SerializeField] private float changeCDMLim = 250; // CDM = collision detection mode

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        netRb = GetComponent<NetworkRigidbody>();
        netObj = GetComponent<NetworkObject>();
    }


    protected virtual void FixedUpdate()
    {
        CheckCDMode();
    }

    private void CheckCDMode()
    {
        magnitude = rb.velocity.magnitude;
        
        if (magnitude >= changeCDMLim)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            return;
        }
        // default
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}