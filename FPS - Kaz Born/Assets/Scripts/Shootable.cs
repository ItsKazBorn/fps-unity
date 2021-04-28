using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    public float health = 10f;
    
    public void TakeDamage(float damageTaken)
    {
        health -= damageTaken;
        if (health <= 0f)
        {
            Die();
        }
    }

    protected virtual void Die() { }

}
