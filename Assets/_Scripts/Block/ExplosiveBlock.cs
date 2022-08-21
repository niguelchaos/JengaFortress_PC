using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBlock : Block
{   
    public GameObject explosionVFX;
    [SerializeField] private float expForce = 300f;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float upModifier = 3f;

    [SerializeField] private bool hasExploded = false;
    [SerializeField] private bool isExploding = false;

    // timer not working
    // [SerializeField] private float timer = 0;
    // private float delay = 2f;


    private void Update()
    {
        // UpdateCountdown();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        // needs to be in fixedupdate because for some reason the force is applied across frames
        Explode();
    }

    private void Explode()
    {
        if (isExploding && !hasExploded)
        {
            // put all affected colliders in a list
            Vector3 explosionPos = transform.position;
            Collider[] affectedColliders = Physics.OverlapSphere(explosionPos, radius);
            // go through list, apply explosion force to each affected block
            foreach (Collider nearbyObject in affectedColliders)
            {
                Rigidbody affectedRb = null;

                if (nearbyObject.gameObject.layer == EditorConstants.BLOCK_LAYER)
                {
                    affectedRb = nearbyObject.GetComponent<Rigidbody>();
                }
                
                if (affectedRb != null && affectedRb != this.rb)
                {
                    // Debug.Log("exploooode");
                    affectedRb.AddExplosionForce(expForce, explosionPos, radius, upModifier, ForceMode.Impulse);
                }

            }
            hasExploded = true;
        }
    }

    // private void CheckExplode()
    // {
    //     if (timer <= 0 && !hasExploded)
    //     {
    //         Explode();
    //         hasExploded = true;
    //         // timer = 0;
    //     }
    // }

    // private void UpdateCountdown()
    // {
    //     if (timer > 0)
    //     {
    //         timer -= Time.deltaTime;
    //     }
    // }

    private void OnCollisionEnter(Collision col)
    {
        // timer = delay;
        if (!hasExploded)
        {
            // compare layer to block + ground layer
            if (((1 << col.gameObject.layer) & LayerMask.GetMask(EditorConstants.BLOCK_LAYER_NAME, EditorConstants.GROUND_LAYER_NAME)) != 0)
            {
                GameObject explosionVFXObj = Instantiate(explosionVFX, transform.position, Quaternion.identity);
                Destroy(explosionVFXObj, explosionVFXObj.GetComponent<ParticleSystem>().main.duration);
                isExploding = true;
            }
        }
    }
}