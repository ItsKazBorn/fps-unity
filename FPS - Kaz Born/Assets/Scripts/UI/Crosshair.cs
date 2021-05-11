using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    private RectTransform reticle;

    [SerializeField] private float restingSize = 50f;
    private float maxSize = 250f;
    [SerializeField] private float speed = 500f;
    private float currentSize;

    private float currentSpread;    
    
    // Start is called before the first frame update
    void Start()
    {
        reticle = GetComponent<RectTransform>();
        GameEvents.current.onSpreadChanged += OnSpreadChanged;
    }

    private void OnDestroy()
    {
        GameEvents.current.onSpreadChanged -= OnSpreadChanged;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSpread > 0)
        {
            float size = restingSize + (currentSpread * 1000);
            size = Mathf.Clamp(size, restingSize, maxSize);
            currentSize = Mathf.Lerp(currentSize, size, Time.deltaTime * speed);
        }
        else
            currentSize = Mathf.Lerp(currentSize, restingSize, Time.deltaTime * speed);
        
        reticle.sizeDelta = new Vector2(currentSize, currentSize);
    }

    void OnSpreadChanged(float spread)
    {
        currentSpread = spread;
    }
}
