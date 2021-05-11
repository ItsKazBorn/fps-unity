using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputEvents : MonoBehaviour
{
    public static InputEvents current;

    private void Awake()
    {
        current = this;
    }

    
    // >>>--------> WEAPONS
    public event Action onFirePressed;
    public void FirePressed()
    {
        if (onFirePressed != null)
        {
            onFirePressed();
        }
    }

    public event Action onFireReleased;
    public void FireReleased()
    {
        if (onFireReleased != null)
        {
            onFireReleased();
        }
    }

    public event Action onReloadPressed;
    public void ReloadPressed()
    {
        if (onReloadPressed != null)
        {
            onReloadPressed();
        }
    }

    public event Action<int> onWeaponSelectPress;
    public void WeaponSelectPress(int index)
    {
        if (onWeaponSelectPress != null)
        {
            onWeaponSelectPress(index);
        }
    }

    public event Action<float> onScrollWheel;
    public void ScrollWheel(float axis)
    {
        if (onScrollWheel != null)
        {
            onScrollWheel(axis);
        }
    }

    public event Action onAimPress;
    public void AimPress()
    {
        if (onAimPress != null)
        {
            onAimPress();
        }
    }

    public event Action onAimRelease;
    public void AimRelease()
    {
        if (onAimRelease != null)
        {
            onAimRelease();
        }
    }

    public event Action<GunType> onAmmoPickUp;
    public void AmmoPickUp(GunType gunType)
    {
        if (onAmmoPickUp != null)
        {
            onAmmoPickUp(gunType);
        }
    }
}
