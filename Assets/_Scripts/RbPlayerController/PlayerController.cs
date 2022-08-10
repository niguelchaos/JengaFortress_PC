using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : PlayerClient
{   
    [Header("GroundDetection")]
    private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    private float groundRayExtraLength = 0.4f;
    private float groundDist = 0.4f;

    private bool flyModePressed = false;

    private void Start()
    {
        FindPlayerHeight();
        this.groundMask = LayerMask.GetMask(EditorConstants.LAYER_GROUND, EditorConstants.LAYER_BLOCK);
        Player.rb.freezeRotation = true;
    }

    private void Update()
    {
        ReceiveInput();

        CheckIsGrounded();
        CheckIsFalling();

        HandleFlyInput();
    }

    private void CheckIsGrounded()
    {
        float raycastLength = Player.playerHeight + groundRayExtraLength;
        // isGrounded = Physics.Raycast(transform.position, Vector3.down, raycastLength);
        // bottom of player
        Player.isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);
    }

    private void CheckIsFalling()
    {
        if (!Player.isGrounded && Player.rb.velocity.y < 0.1f)
        {
            Player.isFalling = true;
        }
        else {
            Player.isFalling = false;
        }
    }

    private void HandleFlyInput()
    {
        if (InputManager.Instance.flyModeInput && !flyModePressed)
        {
            Player.flyMode = !Player.flyMode;
            flyModePressed = true;
        }
    }

    private void FindPlayerHeight()
    {
        // float tempPlayerHeight = Mathf.Infinity;
        // RaycastHit hit;
 
        // if (Physics.Raycast(transform.position, Vector3.down, out hit))
        // {
        //     Player.playerHeight = Vector3.Distance(hit.point, transform.position);
        // }
        Player.playerHeight = 2;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheck.position, groundDist);
    }

    public void ReceiveInput()
    {
        // player has let go of input
        if (InputManager.Instance.flyModeInput == false)
        {
            flyModePressed = false;
        }
    }



}