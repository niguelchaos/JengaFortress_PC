using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

// The gun itself
public class Gun : NetworkBehaviour
{
    private enum State { Idle, Firing, Reloading}
    [SerializeField] private State state = State.Idle;

    [SerializeField] private GunData gunData; 
    [SerializeField] private GameObject muzzle;
    
    [SerializeField] private float timeSinceLastShot; 
    [SerializeField] private float fireRateTime; 
    [SerializeField] private float currentReloadTime; 

    [SerializeField] private GameObject projectile; 

    public static Action<GameObject> propelActionEvent;
    
    private bool subEventsOnStart = true;


    void Awake()
    {
        currentReloadTime = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        // rpm / 60 seconds = rounds per second
        // 1 second / rps = time passed between each shot
        fireRateTime = 1f / (gunData.fireRate / 60f);

        if (subEventsOnStart)
        {
            PlayerGunstick.fireActionEvent += Shoot;
            PlayerGunstick.reloadActionEvent += StartReload;
        }


    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        UnsubEvents();
    }


    void Update()
    {
        Debug.DrawRay(muzzle.transform.position, muzzle.transform.forward, Color.magenta);

        switch(state)
        {
            case State.Idle:
                state = UpdateIdleState();
                break;
            case State.Firing:
                state = UpdateFiringState();
                break;
            case State.Reloading:
                state = UpdateReloadState();
                break;
        }
    }

    private void FixedUpdate()
    {

    }



    private State UpdateIdleState()
    {
        return State.Idle;
    }

    private State UpdateFiringState()
    {
        // fire rate limit
        if (timeSinceLastShot < 0)
        {
            return State.Idle;
        }
        else 
        {
            timeSinceLastShot -= Time.deltaTime;
            return State.Firing;
        }
    }
    
    private State UpdateReloadState()
    {
        // reloading done
        if (currentReloadTime <= 0)
        {
            gunData.currentAmmo = gunData.magSize;
            return State.Idle;
        }
        else 
        {
            currentReloadTime -= Time.deltaTime;
            return State.Reloading;
        }
    }

    private void Shoot()
    {
        if (CanFire())
        {
            // raycast type fire
            bool fireRay = Physics.Raycast(muzzle.transform.position, transform.forward, out RaycastHit hitInfo, gunData.maxDistance);
            if (fireRay)
            {
                // Debug.Log(hitInfo.transform.name);
                IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                damageable?.TakeDamage(gunData.damage);
            }

            gunData.currentAmmo--;
            timeSinceLastShot = fireRateTime;
            state = State.Firing;

            OnGunShot();
        }
    }

    private void OnGunShot()
    {
        // fire for ourselves first
        FireProjectile(muzzle.transform.position, gunData.fireBlockForce);
        // print("Firing for Self ");

        // request server: i just pressed button, spawn projectile from location
        RequestGunshotServerRpc(muzzle.transform.position, gunData.fireBlockForce);

        propelActionEvent?.Invoke(muzzle);
    }

    private void FireProjectile(Vector3 dir, float fireForce)
    {
        Transform spawnTransform = muzzle.transform;
        GameObject spawnedProjectile = Instantiate(projectile, spawnTransform.position, spawnTransform.rotation);
        spawnedProjectile.GetComponent<Rigidbody>().AddForce(spawnTransform.forward * gunData.fireBlockForce, ForceMode.Impulse);
    }

    private bool CanFire()
    {
        if (gunData.currentAmmo > 0)
        {
            if (state != State.Firing || state != State.Reloading)
            {
                return true;
            }
            return false;
        }
        else {
            return false;
        }
    } 

    private void StartReload()
    {
        if (CanReload())
        {
            currentReloadTime = gunData.reloadTime;
            state = State.Reloading;
        }
    }

    private bool CanReload()
    {
        if (state != State.Reloading)
        {
            return true;
        }
        return false;
    }

    public void UnsubEvents()
    {
        PlayerGunstick.fireActionEvent -= Shoot;
        PlayerGunstick.reloadActionEvent -= StartReload;
        subEventsOnStart = false;
    }


    [ServerRpc]
    private void RequestGunshotServerRpc(Vector3 dir, float fireForce)
    {
        // server then calls client to execute
        FireClientRpc(dir, fireForce);
    }

    // can only be called by server
    // code within executed on all clients
    [ClientRpc]
    private void FireClientRpc(Vector3 dir, float fireForce)
    {
        // from all the clients, will now execute shot
        // spawn local version of projectile on everybodys client
       
        if (!IsOwner)
        {
            FireProjectile(dir, fireForce);
        }
    }

}
