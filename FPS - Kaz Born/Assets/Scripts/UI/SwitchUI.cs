using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchUI : MonoBehaviour
{
    [SerializeField] private GameObject _inGameUI;
    [SerializeField] private GameObject _gameOverUI;
    
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.onGameOver += OnGameOver;
    }

    private void OnDestroy()
    {
        GameEvents.current.onGameOver -= OnGameOver;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGameOver()
    {
        _inGameUI.SetActive(false);
        _gameOverUI.SetActive(true);
    }
}
