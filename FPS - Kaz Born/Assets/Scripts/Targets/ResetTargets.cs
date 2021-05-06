using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTargets : MonoBehaviour
{
    public Camera cam;
    public Animator animator;
    public int distance = 3;

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
                    
                    Target[] targets = FindObjectsOfType<Target>();

                    for (int i = 0; i < targets.Length; i++)
                    {
                        targets[i].Reset();
                    }
                }
            }
        }
    }
}
