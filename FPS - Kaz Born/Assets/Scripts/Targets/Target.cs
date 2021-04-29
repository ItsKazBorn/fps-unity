using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : Shootable
{
    private bool shot = false;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected override void Die()
    {
        if (!shot)
        {
            shot = true;
            animator.SetBool("Shot", true);
        }
    }

    public void Reset()
    {
        shot = false;
        animator.SetBool("Shot", false);
    }
}
