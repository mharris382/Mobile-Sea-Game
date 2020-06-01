using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsPlayerHidden { get; set; }
    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        IsPlayerHidden = false;
    }
    
}
