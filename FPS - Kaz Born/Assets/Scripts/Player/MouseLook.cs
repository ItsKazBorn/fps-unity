using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;

    [SerializeField] private Transform playerBody;

    private float xRotation = 0f;

    private bool isGameOver = false;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GameEvents.current.onGameOver += OnGameOver;
    }

    private void OnDestroy()
    {
        GameEvents.current.onGameOver -= OnGameOver;
    }

    void Update()
    {
        if (!isGameOver)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    private void OnGameOver()
    {
        isGameOver = true;
    }
}
