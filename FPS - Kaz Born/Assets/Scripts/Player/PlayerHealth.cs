using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float health = 100f;

    public bool isGameOver = false;

    private void Start()
    {
        health = maxHealth;
        GameEvents.current.PlayerHealthChanged(health, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        GameEvents.current.PlayerHealthChanged(health, maxHealth);
        if (health <= 0)
            Die();
    }

    void Die()
    {
        GameEvents.current.GameOver();
    }

    private void Update()
    {
        if (isGameOver)
        {
            Die();
        }
    }
}
