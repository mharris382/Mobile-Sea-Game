using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static int Treasure { get; private set; }
        
        [SerializeField] private GameObject diverPrefab;
        
        private ObservedValue<bool> _isPlayerHidden;
        private ObservedValue<bool> _isDisruptorActive;
        
        private Level _levelLayout;
        private GameObject _diverGO;
        
        public Level CurrentLevel => _levelLayout ? _levelLayout : (_levelLayout = FindObjectOfType<Level>());

        public IntUnityEvent OnLootPickup;
        
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

                OnLootPickup = new IntUnityEvent();
                
                //TODO: find and store the diver's gameObject inside Awake
                
                _isPlayerHidden = new ObservedValue<bool>(false);
                _isDisruptorActive = new ObservedValue<bool>(false);
                _isDisruptorActive.OnValueChanged += b => OnDisruptorChanged?.Invoke(b) ;
                _isPlayerHidden.OnValueChanged += b => OnPlayerHiddenChanged?.Invoke(b);
                _levelLayout = GameObject.FindObjectOfType<Level>();
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            ResetLevel();
        }


        public void ResetLevel()
        {
            //TODO: Reload the actual scene
            
            _isPlayerHidden.Value = false;
            _isDisruptorActive.Value = false;
            if(_diverGO != null)
                GameObject.Destroy(_diverGO);
            _diverGO = GameObject.Instantiate(diverPrefab, _levelLayout.diverSpawnPosition, Quaternion.identity);
            
        }


        public void EatDiver(string deathMessage = null)
        {
            if (deathMessage != null)
            {
                //TODO: display a death message to the player
                Debug.Log(deathMessage.InBold());
            }
            
            //hide diver now that they have been eaten
            _isPlayerHidden.Value = true;
            
            
            //TODO: disable the diver's gameObject so that the player can no longer control them
            _diverGO.SetActive(false);
            Invoke("ResetLevel", 3);
        }

        public void PickupLoot(int value)
        {
            Treasure += value;
            OnLootPickup?.Invoke(Treasure);
        }
    }

    public class IntUnityEvent : UnityEvent<int>
    {
        
    }
}