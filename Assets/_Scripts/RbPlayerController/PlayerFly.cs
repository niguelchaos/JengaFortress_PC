using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFly : PlayerClient
{
    private enum State { Idle, Fly, Sprint}
    [SerializeField] private State state = State.Idle;

    [SerializeField] private PlayerMovementData MoveData;


    [Header("Movement")]
    [SerializeField] private Vector3 moveVel;
    private float moveSpeed;
    private Vector3 moveDir;

    [Space]
    private RaycastHit slopeHit;
    private Vector3 slopeMoveDir;

    [Space]
    [SerializeField] private Transform orientation;

    
    // Update is called once per frame
    private void Update()
    {
        if (Player.flyMode)
        {
            Player.isOnSlope = OnSlope();
            switch (state)
            {
                case State.Idle:
                    state = UpdateIdleState();
                    break;
                case State.Fly:
                    state = UpdateFlyState();
                    break;
                case State.Sprint:
                    state = UpdateSprintState();
                    break;
            }
            CheckSlopeMoveDir();
            ControlDrag();
        } 
        else {
            state = State.Idle;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Fly:
                Fly();
                break;
            case State.Sprint:
                Sprint();
                break;
        }
    }
    
    private State UpdateIdleState()
    {   
        State currentState = CheckState();
        return currentState;
    }

    private State UpdateFlyState()
    {
        moveSpeed = Mathf.Lerp(moveSpeed, MoveData.walkSpeed, MoveData.accelSpeed * Time.deltaTime);

        State currentState = CheckState();
        return currentState;
    }
    private State UpdateSprintState()
    {
        moveSpeed = Mathf.Lerp(moveSpeed, MoveData.sprintSpeed, MoveData.accelSpeed * Time.deltaTime);

        State currentState = CheckState();
        return currentState;
    }

    private void Fly()
    {
        Move();
    }
    private void Sprint()
    {
        Move();
    }

    private State CheckState()
    {
        if ( !InputManager.Instance.isSprinting && InputManager.Instance.moveDir != Vector2.zero)
        {
            return State.Fly;
        }
        else if ( InputManager.Instance.isSprinting)
        {
            return State.Sprint;
        }
        else {
            return State.Idle;
        }
    }



    private void Move()
    {
        this.moveDir = orientation.forward * InputManager.Instance.moveDir.y + orientation.right * InputManager.Instance.moveDir.x;
        
        CheckVel();
        
        Player.rb.AddForce(moveVel, ForceMode.Acceleration);
    }

    private void CheckGroundedVel()
    {
        if (Player.isGrounded)
        {
            if (!Player.isOnSlope)
            {
                this.moveVel = moveDir.normalized * moveSpeed;
                Debug.DrawRay(transform.position, moveDir.normalized * 20, Color.cyan);
            }
            else if (Player.isOnSlope)
            {
                this.moveVel = slopeMoveDir.normalized * moveSpeed;
                Debug.DrawRay(transform.position, slopeMoveDir.normalized * 20, Color.green);
            }
        }
    }

    private void CheckVel()
    {
        this.moveVel = moveDir.normalized * moveSpeed * MoveData.airMultiplier;
    }

    private void ControlDrag()
    {
        if (Player.isGrounded)
        {
            Player.rb.drag = MoveData.groundDrag;
        }
        else if (!Player.isGrounded) {
            Player.rb.drag = MoveData.airDrag;
        }
    }

    private void CheckSlopeMoveDir()
    {
        slopeMoveDir = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);
    }

    private bool OnSlope()
    {
        // bool slopeRaycast = Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerController.playerHeight / 2 + 0.5f);
        bool slopeRaycast = Physics.Raycast(transform.position, Vector3.down, out slopeHit, Player.playerHeight + MoveData.slopeRayExtraLength);
        Debug.DrawRay(transform.position, Vector3.down * (Player.playerHeight + MoveData.slopeRayExtraLength), Color.black);

        if (slopeRaycast) 
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            } 
            else 
            {
                return false;
            }
        }
        return false;
    }


}
