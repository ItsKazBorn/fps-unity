using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    [SerializeField] private float health = 10f;
    protected bool hasDied = false;
    
    public void TakeDamage(float damageTaken)
    {
        health -= damageTaken;
        if (health <= 0f && !hasDied)
        {
            hasDied = true;
            Die();
        }
    }

    protected virtual void Die() { }

}
