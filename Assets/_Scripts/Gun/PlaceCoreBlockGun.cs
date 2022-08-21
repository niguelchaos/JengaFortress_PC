using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

// places the core block
public class PlaceCoreBlockGun : NetworkBehaviour
{
    private enum State { Idle, Place, Move }
    [SerializeField] private State state = State.Idle;

    [SerializeField] private GunData gunData; 
    [SerializeField] private GameObject muzzle;
    
    [SerializeField] private GameObject CBPrefab;
    private GameObject cb;
    
    private bool subEventsOnStart = true;


    void Start()
    {
        if (subEventsOnStart)
        {
            PlayerPlaceCoreBlock.StartPlaceCoreBlock += BeginPreview;
            PlayerPlaceCoreBlock.PlacedCoreBlock += PlaceCoreBlock;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        UnsubEvents();
    }

    void Update()
    {
        switch(state)
        {
            case State.Idle:
                state = UpdateIdleState();
                break;
            case State.Place:
                state = UpdatePlaceState();
                break;
            case State.Move:
                state = UpdateMoveState();
                break;
        }
    }


    private State UpdateIdleState()
    {
        return State.Idle;
    }

    private State UpdatePlaceState()
    {

        return State.Place;
    }

    private void BeginPreview()
    {
        if (cb == null)
        {
            PreviewPlacement();
        }
        state = State.Place;
    }

    private void PreviewPlacement()
    {
        cb = Instantiate(CBPrefab); 
        cb.GetComponent<BoxCollider>().enabled = false;

        Color blockColor = cb.GetComponent<MeshRenderer>().material.color;
        blockColor = new Color(blockColor.r, blockColor.g, blockColor.b, 0.5f);

        // attach to gun muzzle, raycast to nearest face?
        cb.transform.SetParent(muzzle.transform);
    }

    private State UpdateMoveState()
    {
        return State.Move;
    }

    private void PlaceCoreBlock()
    {
        if (CanPlace())
        {
            cb.GetComponent<BoxCollider>().enabled = true;

            Color blockColor = cb.GetComponent<MeshRenderer>().material.color;
            blockColor = new Color(blockColor.r, blockColor.g, blockColor.b, 1f);
            
            state = State.Move;

            OnPlaceBlock();
        }
    }

    private void OnPlaceBlock()
    {
        // place for ourselves first

        // until confirmed location, do not show on opponents screen
    }

    // private void FireProjectile(Vector3 dir, float fireForce)
    // {
    //     Transform spawnTransform = muzzle.transform;
    //     GameObject spawnedProjectile = Instantiate(projectile, spawnTransform.position, spawnTransform.rotation);
    //     spawnedProjectile.GetComponent<Rigidbody>().AddForce(spawnTransform.forward * gunData.fireBlockForce, ForceMode.Impulse);
    // }

    private bool CanPlace()
    {
        return true;
    } 

    public void UnsubEvents()
    {
        PlayerPlaceCoreBlock.StartPlaceCoreBlock -= BeginPreview;
        PlayerPlaceCoreBlock.PlacedCoreBlock -= PlaceCoreBlock;
        subEventsOnStart = false;
    }


    // [ServerRpc]
    // private void RequestGunshotServerRpc(Vector3 dir, float fireForce)
    // {
    //     // server then calls client to execute
    //     FireClientRpc(dir, fireForce);
    // }

    // // can only be called by server
    // // code within executed on all clients
    // [ClientRpc]
    // private void FireClientRpc(Vector3 dir, float fireForce)
    // {
    //     // from all the clients, will now execute shot
    //     // spawn local version of projectile on everybodys client
       
    //     if (!IsOwner)
    //     {
    //         FireProjectile(dir, fireForce);
    //     }
    // }

}
