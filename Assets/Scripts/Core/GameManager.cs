using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
        [SerializeField] private int currentScene = 0;

        public bool dontLoadScene = true;
        private ObservedValue<bool> _isPlayerHidden;
        private ObservedValue<bool> _isDisruptorActive;

        private Level _levelLayout;
        private GameObject _diverGO;

        public GameObject CurrentDiver => _diverGO;

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


        public Level CurrentLevel => _levelLayout ? _levelLayout : (_levelLayout = FindObjectOfType<Level>());

        //TODO: Refactor this setup for dummy divers
        public static event Action<bool> OnPlayerHiddenChanged;
        public static event Action<bool> OnDisruptorChanged;
        public static event Action<string> OnPlayerKilled;
        public static event Action<Level> OnLevelLoad;

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
                _isDisruptorActive.OnValueChanged += b => OnDisruptorChanged?.Invoke(b);
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
            if(!dontLoadScene)
            ResetLevel();
        }


        public void ResetLevel()
        {
            _isPlayerHidden.Value = false;
            _isDisruptorActive.Value = false;


            StartCoroutine(loadScene());
            return;
            SceneManager.LoadScene(currentScene);

            if (_diverGO != null) GameObject.Destroy(_diverGO);
            _diverGO = GameObject.Instantiate(diverPrefab, _levelLayout.diverSpawnPosition, Quaternion.identity);


            // SceneManager.MoveGameObjectToScene(_diverGO, SceneManager.GetActiveScene());


            //OnLevelReset?.Invoke();
        }

        private void InitDiverCamera()
        {
            var vcam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
            vcam.m_Follow = _diverGO.transform;
        }

        IEnumerator loadScene()
        {
            yield return SceneManager.LoadSceneAsync(currentScene);


            _levelLayout = FindObjectOfType<Level>();
            if (_levelLayout == null)
                Debug.LogWarning("GameManager loaded into a scene without a Level Object");
            OnLevelLoad?.Invoke(_levelLayout);
            if (_diverGO == null)
                _diverGO = GameObject.FindGameObjectWithTag("Player");
            _diverGO.transform.position = _levelLayout.diverSpawnPosition;
            // if (_diverGO != null) GameObject.Destroy(_diverGO);
            // _diverGO = GameObject.Instantiate(diverPrefab, _levelLayout.diverSpawnPosition, Quaternion.identity);
            //
            // InitDiverCamera();
        }


        public void KillDiver(string deathMessage = null)
        {
            //Notify listeners that player was killed
            deathMessage = String.IsNullOrEmpty(deathMessage) ? "Diver was killed!" : deathMessage;
            OnPlayerKilled?.Invoke(deathMessage);

            //hide diver now that they have been eaten
            _isPlayerHidden.Value = true;

            _diverGO.SetActive(false);

#if UNITY_EDITOR
            if (FindObjectOfType<GameOverScreen>() == null)
            {
                Debug.LogError("No GameOverScreen is in the scene, the scene will not be reset!");
            }
#endif
        }

        public void KillDiver(GameObject diverToKill, string deathMessage = null)
        {
            if (diverToKill != null && diverToKill != _diverGO)
            {
                Debug.Log("It wasn't the real diver that was killed!".InBold());
                return;
            }

            KillDiver(deathMessage);
        }


        public void PickupLoot(int value)
        {
            Treasure += value;
            OnLootPickup?.Invoke(Treasure);
        }


        public interface IDiverCamera
        {
            void FollowNewDiver(GameObject diverGameObject);
        }
    }


    public class IntUnityEvent : UnityEvent<int>
    {
    }
}