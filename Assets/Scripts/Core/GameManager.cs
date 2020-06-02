using System;
using System.Collections;
using System.Collections.Generic;
using Core.Utilities;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    private ObservedValue<bool> _isPlayerHidden;
    private ObservedValue<bool> _isDisruptorActive;
    public bool IsPlayerHidden
    {
        get => _isPlayerHidden.Value;
        set => _isPlayerHidden.Value = value;
    }

    public bool IsDisruptorActive
    {
        get => _isDisruptorActive.Value;
        set => _isDisruptorActive.Value = value;
    }

    public static event Action<bool> OnPlayerHiddenChanged;
    public static event Action<bool> OnDisruptorChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            _isPlayerHidden = new ObservedValue<bool>(false);
            _isDisruptorActive = new ObservedValue<bool>(false);
            _isDisruptorActive.OnValueChanged += b => OnDisruptorChanged?.Invoke(b) ;
            _isPlayerHidden.OnValueChanged += b => OnPlayerHiddenChanged?.Invoke(b);
        }
        else
        {
            Destroy(this);
        }
    }

    
}

