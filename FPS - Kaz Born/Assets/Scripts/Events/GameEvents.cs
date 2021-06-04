using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }
    
    public event Action onQPressed;
    public void QPressed()
    {
        if (onQPressed != null)
            onQPressed();
    }
    
    public event Action onQReleased;
    public void QReleased()
    {
        if (onQReleased != null)
            onQReleased();
    }


    public event Action<int, int> onAmmoChanged;
    public void AmmoChanged(int inMag, int stored)
    {
        if (onAmmoChanged != null)
            onAmmoChanged(inMag, stored);
    }

    public event Action<float> onSpreadChanged;
    public void SpreadChanged(float spread)
    {
        if (onSpreadChanged != null)
            onSpreadChanged(spread);
    }

    public event Action onWeaponFire;
    public void WeaponFire()
    {
        if (onWeaponFire != null)
            onWeaponFire();
    }

    public event Action onGameOver;
    public void GameOver()
    {
        if (onGameOver != null)
            onGameOver();
    }

    public event Action onGameReset;
    public void GameReset()
    {
        if (onGameReset != null)
            onGameReset();
    }

    public event Action<float, float> onPlayerHealthChanged;
    public void PlayerHealthChanged(float currentHealth, float maxHealth)
    {
        if (onPlayerHealthChanged != null)
            onPlayerHealthChanged(currentHealth, maxHealth);
    }

    public event Action onSoundFired;
    public void SoundFired()
    {
        if (onSoundFired != null)
            onSoundFired();
    }
}
