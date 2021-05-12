using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CameraRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private float rotationSpeed = 6;
    [SerializeField] private float returnSpeed = 25;
    [SerializeField] private Vector3 recoilRotation = new Vector3(2f, 2f, 2f);

    private bool aiming;
    private Vector3 currentRotation;
    private Vector3 rot;

    private void FixedUpdate()
    {
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        rot = Vector3.Slerp(rot, currentRotation, rotationSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        currentRotation += new Vector3(-recoilRotation.x, UnityEngine.Random.Range(-recoilRotation.y, recoilRotation.y),
            UnityEngine.Random.Range(-recoilRotation.z, recoilRotation.z));
        
    }

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.onWeaponFire += Fire;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
