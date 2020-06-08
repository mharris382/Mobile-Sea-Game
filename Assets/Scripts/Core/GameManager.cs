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
        //TODO: change this to an int (build index)
        public int currentScene = 0;

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
        public static event Action<string> OnPlayerKilled;
        public static event Action OnLevelReset;
    
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
            if (Application.isMobilePlatform)
                QualitySettings.vSyncCount = 0;
            ResetLevel();
        }


        public void ResetLevel()
        {
            
            _isPlayerHidden.Value = false;
            _isDisruptorActive.Value = false;


            SceneManager.LoadScene(currentScene);

            if (_diverGO != null) GameObject.Destroy(_diverGO);
            _diverGO = GameObject.Instantiate(diverPrefab, _levelLayout.diverSpawnPosition, Quaternion.identity);
            Debug.Log(_diverGO);
            DontDestroyOnLoad(_diverGO);
           // SceneManager.MoveGameObjectToScene(_diverGO, SceneManager.GetActiveScene());


            //OnLevelReset?.Invoke();

        }
        IEnumerator loadScene()
        {
            yield return SceneManager.LoadSceneAsync(currentScene);
            if (_diverGO != null) GameObject.Destroy(_diverGO);
            _diverGO = GameObject.Instantiate(diverPrefab, _levelLayout.diverSpawnPosition, Quaternion.identity);
        }

        public void EatDiver(string deathMessage = null)
        {
            //Notify listeners that player was killed
            deathMessage = String.IsNullOrEmpty(deathMessage) ? "Diver was killed!" : deathMessage;
            OnPlayerKilled?.Invoke(deathMessage);

            //hide diver now that they have been eaten
            _isPlayerHidden.Value = true;

            _diverGO.SetActive(false);

#if UNITY_EDITOR
            if(FindObjectOfType<GameOverScreen>() == null)
            {
                Debug.LogError("No GameOverScreen is in the scene, the scene will not be reset!");
            }
#endif
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