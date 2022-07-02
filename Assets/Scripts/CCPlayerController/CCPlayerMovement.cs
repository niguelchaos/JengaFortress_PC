using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCPlayerMovement : MonoBehaviour
{   
    public CharacterController charController;
    public float speed = 15f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    bool isGrounded;

    Vector3 velocity;

    private void Start()
    {

    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");
        bool jumpInput = Input.GetButtonDown("Jump");
        
        Vector3 direction = (transform.right * xInput) + (transform.forward * zInput);
        Vector3 moveVel = direction * speed * Time.deltaTime;
        charController.Move(moveVel);

        if(jumpInput && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        charController.Move(velocity * Time.deltaTime);
    }
}