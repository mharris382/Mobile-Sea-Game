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
    public bool IsPlayerHidden
    {
        get => _isPlayerHidden.Value;
        set => _isPlayerHidden.Value = value;
    }

    public static event Action<bool> OnPlayerHiddenChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            _isPlayerHidden = new ObservedValue<bool>(false);
            _isPlayerHidden.OnValueChanged += OnPlayerHiddenChanged;
        }
        else
        {
            Destroy(this);
        }
    }
    
}

