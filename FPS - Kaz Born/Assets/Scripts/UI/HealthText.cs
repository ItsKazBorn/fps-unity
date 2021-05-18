using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthText : MonoBehaviour
{
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        GameEvents.current.onPlayerHealthChanged += SetHealth;
    }

    private void OnDestroy()
    {
        GameEvents.current.onPlayerHealthChanged -= SetHealth;
    }

    void SetHealth(float health, float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = health;
    }

}
