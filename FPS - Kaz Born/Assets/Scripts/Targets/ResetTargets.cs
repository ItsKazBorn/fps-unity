using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTargets : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Animator animator;
    [SerializeField] private int distance = 3;
    private Target[] _targets;

    private void Start()
    {
        _targets = FindObjectsOfType<Target>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();

                if (interactable != null)
                {
                    animator.SetTrigger("Pressed");

                    for (int i = 0; i < _targets.Length; i++)
                    {
                        _targets[i].Reset();
                    }
                }
            }
        }
    }
}
