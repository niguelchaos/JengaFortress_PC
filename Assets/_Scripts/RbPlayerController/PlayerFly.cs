using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFly : PlayerClient
{
    private enum State { Idle, Fly, Sprint}
    [SerializeField] private State state = State.Idle;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private const float walkSpeed = 6f;
    [SerializeField] private const float sprintSpeed = 24f;
    [SerializeField] private const float accelSpeed = 10f;

    // [SerializeField] private float groundMultiplier = 10f;
    // [SerializeField] private float airMultiplier = 0.4f;
    [SerializeField] private Vector3 moveVel;
    private Vector3 moveDir;


    [Space]
    private RaycastHit slopeHit;
    private float slopeRayExtraLength = 1f;
    public Vector3 slopeMoveDir;

    // [Header("Drag")]
    [Space]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 6f;

    [Space]
    [SerializeField] Transform orientation;


    private void Start()
    {
        
    }


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
        moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, accelSpeed * Time.deltaTime);

        State currentState = CheckState();
        return currentState;
    }
    private State UpdateSprintState()
    {
        moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, accelSpeed * Time.deltaTime);

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

    private void CheckAirVel()
    {
        // in the air
        if (!Player.isGrounded) 
        {
            this.moveVel = moveDir.normalized * moveSpeed;
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
