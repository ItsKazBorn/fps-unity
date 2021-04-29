using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Transform groundCheck;
    public Animator weaponAnimator;

    [Header("Move")]
    public float baseSpeed = 12f;
    private float speed;
    
    [Header("Jump")]
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Vector3 velocity;
    private bool isGrounded = false;
    
    void Update()
    {
        Jump();
        Move();
    }

    void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; 

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void Move()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            speed = baseSpeed * 2f;
        else
            speed = baseSpeed;

        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 moveDir = transform.right * xInput + transform.forward * zInput;

        controller.Move(moveDir * speed * Time.deltaTime);
        weaponAnimator.SetFloat("Magnitude", moveDir.magnitude);
    }
}
