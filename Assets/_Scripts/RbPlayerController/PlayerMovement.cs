using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerClient
{
    private enum State { Idle, Walk, Sprint, Crouch}
    [SerializeField] private State state = State.Idle;

    [SerializeField] private PlayerMovementData MoveData;

    [SerializeField] private Vector3 moveVel;
    [SerializeField] private float moveMag;
    [SerializeField] private float moveSpeed;
    private Vector3 moveDir;

    [Space]
    private RaycastHit slopeHit;
    private Vector3 slopeMoveDir;

    [Space]
    [SerializeField] private Transform orientation;


    // Update is called once per frame
    private void Update()
    {
        Player.isOnSlope = OnSlope();
        // ReceiveInput();
        switch (state)
        {
            case State.Idle:
                state = UpdateIdleState();
                break;
            case State.Walk:
                state = UpdateWalkState();
                break;
            case State.Sprint:
                state = UpdateSprintState();
                break;
            case State.Crouch:
                state = UpdateCrouchState();
                break;
        }
        CheckSlopeMoveDir();
        ControlDrag();

        moveMag = Player.rb.velocity.magnitude;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Walk:
                Walk();
                break;
            case State.Sprint:
                Sprint();
                break;
            case State.Crouch:
                Crouch();
                break;
        }

        // StronkGravity();
    }
    
    private State UpdateIdleState()
    {   
        State currentState = CheckState();
        return currentState;
    }

    private State UpdateWalkState()
    {
        moveSpeed = Mathf.Lerp(moveSpeed, MoveData.walkSpeed, MoveData.accelSpeed * Time.deltaTime);

        State currentState = CheckState();
        return currentState;
    }
    private State UpdateSprintState()
    {
        if (Player.isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, MoveData.sprintSpeed, MoveData.accelSpeed * Time.deltaTime);
        }

        State currentState = CheckState();
        return currentState;
    }
    private State UpdateCrouchState()
    {
        if (Player.isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, MoveData.crouchSpeed, MoveData.accelSpeed * Time.deltaTime);
        }

        State currentState = CheckState();
        return currentState;
    }

    private void Walk()
    {
        Move();
    }
    private void Sprint()
    {
        Move();
    }
    private void Crouch()
    {
        Move();
    }

    private State CheckState()
    {
        if ( !InputManager.Instance.isSprinting && !InputManager.Instance.isCrouching && InputManager.Instance.moveDir != Vector2.zero)
        {
            return State.Walk;
        }
        else if ( InputManager.Instance.isSprinting && !InputManager.Instance.isCrouching)
        {
            return State.Sprint;
        }
        else if ( InputManager.Instance.isCrouching && !InputManager.Instance.isSprinting)
        {
            return State.Crouch;
        }
        else {
            return State.Idle;
        }
    }



    private void Move()
    {
        this.moveDir = orientation.forward * InputManager.Instance.moveDir.y + orientation.right * InputManager.Instance.moveDir.x;
        
        CheckGroundedVel();
        CheckAirVel();

        
        Player.rb.AddForce(moveVel, ForceMode.Acceleration);
    }

    private void CheckGroundedVel()
    {
        if (Player.isGrounded)
        {
            if (!Player.isOnSlope)
            {
                this.moveVel = moveDir.normalized * moveSpeed * MoveData.groundMultiplier;
                Debug.DrawRay(transform.position, moveDir.normalized * 20, Color.cyan);
            }
            else if (Player.isOnSlope)
            {
                this.moveVel = slopeMoveDir.normalized * moveSpeed * MoveData.groundMultiplier;
                Debug.DrawRay(transform.position, slopeMoveDir.normalized * 20, Color.green);
            }
        }
    }

    private void CheckAirVel()
    {
        // in the air
        if (!Player.isGrounded) 
        {
            this.moveVel = moveDir.normalized * moveSpeed * MoveData.airMultiplier;
        }
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
