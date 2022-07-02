using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] private GameObject projectile;    // this is a reference to your projectile prefab

    // [SerializeField] private bool hasFired = false;
    [SerializeField] private float fireForce = 10000;

    // Update is called once per frame
    private void Start()
    {
        // subscribe to onfire
        // InputManager.Instance.OnFireInput += Fire;
    }
    void FixedUpdate()
    {
        // Fire();
    }

    private void Fire(bool fired)
    {
        if (fired)
        {
            Transform spawnTransform = gameObject.transform;
            
            GameObject spawnedProjectile = Instantiate(projectile, spawnTransform.position, spawnTransform.rotation);
            spawnedProjectile.GetComponent<Rigidbody>().AddForce(spawnTransform.forward * fireForce, ForceMode.Impulse);
        }
    }


}
