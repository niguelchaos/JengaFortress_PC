using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : PlayerClient
{
    private enum State { Idle, Wallrunning, StoppingWallrun}
    [SerializeField] private State state = State.Idle;

    [SerializeField] private Vector2 sens;
    private Vector2 mouseInput;
    private Vector2 rotation;
    private Vector2 mousePos;
    private float multiplier = 0.01f;

    [SerializeField] Transform orientation;
    [SerializeField] Transform camHolder;


    [SerializeField] private float fov;
    [SerializeField] private float wallRunFov;
    [SerializeField] private float wallRunFovTime;
    [SerializeField] private float camTilt;
    [SerializeField] private float camTiltTime;

    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // Player.notNetworkOwnerEvent += OnNotNetworkOwner;
    }

    // private void OnNotNetworkOwner()
    // {
    //     Destroy(this);
    // }

    void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    
    private void Update()
    {
        if (Player.cam != null)
        {
            ProcessInput();
            Look();

            switch(state)
            {
                case State.Idle:
                    state = UpdateIdleState();
                    break;
                case State.Wallrunning:
                    state = UpdateWallrunningState();
                    break;
                case State.StoppingWallrun:
                    state = UpdateStoppingWallrunState();
                    break;
            }
        }
    }
    
    public void ProcessInput()
    {
        mouseInput = InputManager.Instance.lookInput;
        mousePos.x = mouseInput.x * sens.x;
        mousePos.y = mouseInput.y * sens.y;
        
        rotation.y += mousePos.x * multiplier;
        rotation.x -= mousePos.y * multiplier;

        // cannot look too far up or down
        rotation.x = Mathf.Clamp(rotation.x, -90f, 90f);
    }

    private void Look()
    {
        // only want player to rotate on y axis
        camHolder.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, Player.currentTilt);
        orientation.transform.rotation = Quaternion.Euler(0, rotation.y, 0);
    }

    private State UpdateIdleState()
    {
        if (Player.isWallRunning)
        {
            return State.Wallrunning;
        }
        else 
        {
            StopWallrunTilt();
            return State.Idle;
        }
    }
    private State UpdateWallrunningState()
    {
        if (Player.isWallRunning)
        {
            StartWallRunTilt();
            return State.Wallrunning;
        } 
        else 
        {
            return State.StoppingWallrun;
        }
    }

    private State UpdateStoppingWallrunState()
    {
        if (Player.isGrounded && !(Player.isWallLeft || Player.isWallRight))
        {
            StopWallrunTilt();
            return State.StoppingWallrun;
        }
        else 
        {
            return State.Idle;
        }
    }

    private void StartWallRunTilt()
    {
        Player.cam.fieldOfView = Mathf.Lerp(Player.cam.fieldOfView, wallRunFov, wallRunFovTime * Time.deltaTime);

        if (Player.isWallLeft)
        {
            Player.currentTilt = Mathf.Lerp(Player.currentTilt, -camTilt, camTiltTime * Time.deltaTime);
        } 
        else if (Player.isWallRight)
        {
            Player.currentTilt = Mathf.Lerp(Player.currentTilt, camTilt, camTiltTime * Time.deltaTime);
        }
    }

    private void StopWallrunTilt()
    {
        Player.cam.fieldOfView = Mathf.Lerp(Player.cam.fieldOfView, fov, wallRunFovTime * Time.deltaTime);
        Player.currentTilt = Mathf.Lerp(Player.currentTilt, 0, camTiltTime * Time.deltaTime);
    }

    



    
}