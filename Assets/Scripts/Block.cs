using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Rigidbody rb {get; private set;}
    [SerializeField] private float magnitude;
    [SerializeField] private float changeCDMLim = 250;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
}