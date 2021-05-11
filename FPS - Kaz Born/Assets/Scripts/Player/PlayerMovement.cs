using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Animator weaponAnimator;
    private Transform _transform;

    [Header("Move")]
    [SerializeField] private float baseSpeed = 12f;
    private float speed;
    
    [Header("Jump")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    private Vector3 velocity;
    private bool isGrounded = false;

    private void Start()
    {
        _transform = GetComponent<Transform>();

        speed = baseSpeed;
        
        InputEvents.current.onSprintPressed += StartSprinting;
        InputEvents.current.onSprintReleased += StopSprinting;
        InputEvents.current.onJumpPressed += Jump;
        InputEvents.current.onWASDPressed += Move;

    }

    private void OnDestroy()
    {
        InputEvents.current.onSprintPressed -= StartSprinting;
        InputEvents.current.onSprintReleased -= StopSprinting;
        InputEvents.current.onJumpPressed -= Jump;
        InputEvents.current.onWASDPressed -= Move;
    }

    void Update()
    {
        Gravity();
    }

    private void Jump()
    {
        if (isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void Gravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; 
        
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void Move(float xInput, float zInput)
    {
        Vector3 moveDir = _transform.right * xInput + _transform.forward * zInput;

        controller.Move(moveDir * speed * Time.deltaTime);
        weaponAnimator.SetFloat("Magnitude", moveDir.magnitude);
    }

    private void StopSprinting()
    {
        speed = baseSpeed;
    }

    private void StartSprinting()
    {
        speed = baseSpeed * 2;
    }
}
