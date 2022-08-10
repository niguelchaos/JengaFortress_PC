using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWallRun : PlayerClient
{
    private enum State { Idle, Wallrunning, Stopping}
    [SerializeField] private State state = State.Idle;

    [SerializeField] Transform orientation;

    [SerializeField] private float wallDist = 0.5f;
    [SerializeField] private float minJumpHeight = 1.5f;
    private LayerMask groundMask;


    
    // public float currentTilt { get; private set; }

    private void Start()
    {
        groundMask = LayerMask.GetMask(EditorConstants.LAYER_GROUND, EditorConstants.LAYER_BLOCK);
        // Player.notNetworkOwnerEvent += OnNotNetworkOwner;
    }

    // private void OnNotNetworkOwner()
    // {
    //     Destroy(this);
    // }

    // void OnDestroy()
    // {
    //     Player.notNetworkOwnerEvent -= OnNotNetworkOwner;
    // }

    private void Update()
    {
        CheckWall();
        switch(state)
        {
            case State.Idle:
                state = UpdateIdleState();
                break;
            case State.Wallrunning:
                state = UpdateWallrunningState();
                break;
            case State.Stopping:
                state = UpdateStoppingState();
                break;
        }
    }

    private void FixedUpdate()
    {
    }

    private State UpdateIdleState()
    {
        if (CanWallRun() && (Player.isWallLeft || Player.isWallRight))
        {
            return State.Wallrunning;
        }
        else 
        {
            StopWallRun();
            return State.Idle;
        }
    }
    private State UpdateWallrunningState()
    {
        if (CanWallRun())
        {
            if (Player.isWallLeft || Player.isWallRight)
            {
                StartWallRun();
                return State.Wallrunning;
            }
            else
            {
                return State.Stopping;
            }
        }
        else 
        {
            return State.Idle;
        }
    }
    private State UpdateStoppingState()
    {
        if (CanWallRun() && !(Player.isWallLeft || Player.isWallRight))
        {
            StopWallRun();
            return State.Stopping;
        }
        else 
        {
            return State.Idle;
        }
    }

    private void CheckWall()
    {
        // rotating orientation when we look
        Player.isWallLeft = Physics.Raycast(transform.position, -orientation.right, out Player.leftWallHit, wallDist, groundMask);
        Player.isWallRight = Physics.Raycast(transform.position, orientation.right, out Player.rightWallHit, wallDist, groundMask);

        Debug.DrawRay(transform.position, -orientation.right * wallDist, Color.green);
        Debug.DrawRay(transform.position, orientation.right * wallDist, Color.cyan);

    }
    
    private bool CanWallRun()
    {
        bool isGroundedRaycast = Physics.Raycast(transform.position, Vector3.down, minJumpHeight);
        return !isGroundedRaycast;
    }

    private void StartWallRun()
    {
        // rb.useGravity = false;
        // rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);
        // print("WALLRUNNING");
        
        Player.isWallRunning = true;
    }

    private void StopWallRun()
    {
        Player.isWallRunning = false;
    }


}