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
        {
            onQPressed();
        }
    }
    
    public event Action onQReleased;
    public void QReleased()
    {
        if (onQReleased != null)
        {
            onQReleased();
        }
    }


    public event Action<int, int> onAmmoChanged;

    public void AmmoChanged(int inMag, int stored)
    {
        if (onAmmoChanged != null)
        {
            onAmmoChanged(inMag, stored);
        }
    }
}
