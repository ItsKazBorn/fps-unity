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
        Gravity();
    }

    public void Jump()
    {
        if (isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void Gravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; 
        
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void Move(float xInput, float zInput)
    {
        Vector3 moveDir = transform.right * xInput + transform.forward * zInput;

        controller.Move(moveDir * speed * Time.deltaTime);
        weaponAnimator.SetFloat("Magnitude", moveDir.magnitude);
    }

    public void Sprint(bool isSprinting)
    {
        if (isSprinting)
            speed = baseSpeed * 2f;
        else
            speed = baseSpeed;
    }
}
