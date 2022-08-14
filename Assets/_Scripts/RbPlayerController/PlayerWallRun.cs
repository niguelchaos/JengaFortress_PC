using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWallRun : PlayerClient
{
    private enum State { Idle, Wallrunning, Stopping}
    [SerializeField] private State state = State.Idle;
    [SerializeField] private PlayerWallRunData WallRunData;

    [SerializeField] Transform orientation;

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
        Player.isWallLeft = Physics.Raycast(transform.position, -orientation.right, out Player.leftWallHit, WallRunData.wallDist, Player.Data.groundMask);
        Player.isWallRight = Physics.Raycast(transform.position, orientation.right, out Player.rightWallHit, WallRunData.wallDist, Player.Data.groundMask);

        Debug.DrawRay(transform.position, -orientation.right * WallRunData.wallDist, Color.green);
        Debug.DrawRay(transform.position, orientation.right * WallRunData.wallDist, Color.cyan);

    }
    
    private bool CanWallRun()
    {
        bool isGroundedRaycast = Physics.Raycast(transform.position, Vector3.down, WallRunData.minJumpHeight);
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