using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : PlayerClient
{
    private enum State { Idle, Wallrunning, StoppingWallrun}
    [SerializeField] private State state = State.Idle;
    [SerializeField] private PlayerLookData LookData;

    [SerializeField] private float currentTilt;

    private Vector2 mouseInput;
    private Vector2 rotation;
    private Vector2 mousePos;

    [SerializeField] Transform orientation;
    // [SerializeField] Transform camHolder;

    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

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
        mousePos.x = mouseInput.x * LookData.sens.x;
        mousePos.y = mouseInput.y * LookData.sens.y;
        
        rotation.y += mousePos.x * LookData.rotMultiplier;
        rotation.x -= mousePos.y * LookData.rotMultiplier;

        // cannot look too far up or down
        rotation.x = Mathf.Clamp(rotation.x, -90f, 90f);
    }

    private void Look()
    {
        // only want player to rotate on y axis
        Player.camHolder.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, currentTilt);
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
        Player.cam.fieldOfView = Mathf.Lerp(Player.cam.fieldOfView, LookData.wallRunFov, LookData.wallRunFovTime * Time.deltaTime);

        if (Player.isWallLeft)
        {
            currentTilt = Mathf.Lerp(currentTilt, -LookData.camTilt, LookData.camTiltTime * Time.deltaTime);
        } 
        else if (Player.isWallRight)
        {
            currentTilt = Mathf.Lerp(currentTilt, LookData.camTilt, LookData.camTiltTime * Time.deltaTime);
        }
    }

    private void StopWallrunTilt()
    {
        Player.cam.fieldOfView = Mathf.Lerp(Player.cam.fieldOfView, LookData.fov, LookData.wallRunFovTime * Time.deltaTime);
        currentTilt = Mathf.Lerp(currentTilt, 0, LookData.camTiltTime * Time.deltaTime);
    }

    



    
}