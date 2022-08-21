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
    [SerializeField] private Vector3 vel;
    private float speed;
    [SerializeField] private Vector3 dir;

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
        speed = Mathf.Lerp(speed, MoveData.flyWalkSpeed, MoveData.accelSpeed * Time.deltaTime);

        State currentState = CheckState();
        return currentState;
    }
    private State UpdateSprintState()
    {
        speed = Mathf.Lerp(speed, MoveData.flySprintSpeed, MoveData.accelSpeed * Time.deltaTime);

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
        if ( !InputManager.Instance.isSprinting && 
                (InputManager.Instance.moveDir != Vector2.zero || InputManager.Instance.jumpHeld || InputManager.Instance.isCrouching ) )
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
        float upDir = 0, downDir = 0;
        if (InputManager.Instance.jumpHeld) upDir = 1;
        if (InputManager.Instance.isCrouching) downDir = 1;

        this.dir = (Player.camHolder.forward * InputManager.Instance.moveDir.y) + 
                    (Player.camHolder.right * InputManager.Instance.moveDir.x) + 
                    (Player.camHolder.up * upDir) +
                    (-Player.camHolder.up * downDir);
        // Debug.DrawRay(Player.camHolder.position, Player.camHolder.forward * 20, Color.blue);
        
        CheckVelocity();
        
        Player.rb.AddForce(vel, ForceMode.Acceleration);
    }

    private void CheckVelocity()
    {
        this.vel = dir.normalized * speed * MoveData.airMultiplier;
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


}
