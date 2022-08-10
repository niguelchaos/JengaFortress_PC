using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerClient
{
    private enum State { Idle, Walk, Sprint, Crouch}
    [SerializeField] private State state = State.Idle;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 24f;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float accelSpeed = 10f;

    [SerializeField] private float groundMultiplier = 10f;
    [SerializeField] private float airMultiplier = 0.4f;
    [SerializeField] private Vector3 moveVel;
    private Vector3 moveDir;


    [Space]
    private RaycastHit slopeHit;
    private float slopeRayExtraLength = 1f;
    public Vector3 slopeMoveDir;

    // [Header("Drag")]
    [Space]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;

    [Space]
    [SerializeField] Transform orientation;


    private void Start()
    {
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
        moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, accelSpeed * Time.deltaTime);

        State currentState = CheckState();
        return currentState;
    }
    private State UpdateSprintState()
    {
        if (Player.isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, accelSpeed * Time.deltaTime);
        }

        State currentState = CheckState();
        return currentState;
    }
    private State UpdateCrouchState()
    {
        if (Player.isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, crouchSpeed, accelSpeed * Time.deltaTime);
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
                this.moveVel = moveDir.normalized * moveSpeed * groundMultiplier;
                Debug.DrawRay(transform.position, moveDir.normalized * 20, Color.cyan);
            }
            else if (Player.isOnSlope)
            {
                this.moveVel = slopeMoveDir.normalized * moveSpeed * groundMultiplier;
                Debug.DrawRay(transform.position, slopeMoveDir.normalized * 20, Color.green);
            }
        }
    }

    private void CheckAirVel()
    {
        // in the air
        if (!Player.isGrounded) 
        {
            this.moveVel = moveDir.normalized * moveSpeed * airMultiplier;
        }
    }

    private void ControlDrag()
    {
        if (Player.isGrounded)
        {
            Player.rb.drag = groundDrag;
        }
        else if (!Player.isGrounded) {
            Player.rb.drag = airDrag;
        }
    }

    private void CheckSlopeMoveDir()
    {
        slopeMoveDir = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);
    }

    private bool OnSlope()
    {
        // bool slopeRaycast = Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerController.playerHeight / 2 + 0.5f);
        bool slopeRaycast = Physics.Raycast(transform.position, Vector3.down, out slopeHit, Player.playerHeight + slopeRayExtraLength);
        Debug.DrawRay(transform.position, Vector3.down * (Player.playerHeight + slopeRayExtraLength), Color.black);

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
