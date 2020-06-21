using System;
using Cinemachine;
using Core;
using Player.Diver;
using Player.Sub;
using PolyNav;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private DiverConfig diverConfig;
        [SerializeField] private GameObject diverPrefab;
        [SerializeField] private SubConfig subConfig;
        [SerializeField] private GameObject subPrefab;
        public CinemachineVirtualCamera diverCam;
        public PolyNav2D map;

        private GameObject _subInstance;
        private GameObject _diverInstance;
        
        private static PlayerController _instance;

        public static PlayerController Instance => _instance;

        public DiverConfig DiverConfig => diverConfig;
        public SubConfig SubConfig => subConfig;
        public GameObject SubInstance => _subInstance;
        public GameObject DiverInstance => _diverInstance;
        public Transform SubNavTarget { get; private set; }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
                GameManager.OnLevelLoad += OnLevelLoaded;
                
            }
            else
            {
                Destroy(this);
            }
        }

        private void SpawnDiver(Vector3 diverSpawnPosition)
        {
            if (_diverInstance != null) GameObject.Destroy(_diverInstance.gameObject);
            diverSpawnPosition.z = 0;
            _diverInstance = GameObject.Instantiate(diverPrefab, diverSpawnPosition, Quaternion.identity);
            _diverInstance.SendMessage("SetConfig", DiverConfig, SendMessageOptions.RequireReceiver);
            diverCam.m_Follow = _diverInstance.transform;
        }

        private void SpawnSub(Vector3 subSpawnPosition)
        {
            if (_subInstance != null) GameObject.Destroy(_subInstance.gameObject);
            subSpawnPosition.z = 0;
            _subInstance = GameObject.Instantiate(subPrefab, subSpawnPosition, Quaternion.identity);

            if (SubNavTarget == null)
            {
                var go = new GameObject("Sub Nav Goal");
                SubNavTarget = go.transform;
            }

            SubNavTarget.position = subSpawnPosition + Vector3.right + (Vector3.down/2f);
            
            _subInstance.SendMessage("SetConfig", SubConfig, SendMessageOptions.RequireReceiver);
            _subInstance.SendMessage("SetTarget", SubNavTarget, SendMessageOptions.RequireReceiver);
            if (map == null)
            {
                var mgo = GameObject.FindGameObjectWithTag("SubNavMap");
                Debug.Assert(mgo != null, "No Map Tag Found");
                map = mgo.GetComponent<PolyNav2D>();
                Debug.Assert(map != null, "No Map found");
            }
            _subInstance.SendMessage("SetMap", map, SendMessageOptions.RequireReceiver);
        }

        private void ResetPlayerEntities()
        {
            if (GameManager.Instance.CurrentLevel == null) return;
            
            Vector2 spawnPoint = GameManager.Instance.CurrentLevel.diverSpawnPosition;
            
            if (DiverInstance == null) SpawnDiver(spawnPoint);
            DiverInstance.transform.position = spawnPoint;
            
            spawnPoint += (Vector2.right * 3);
            
            if (_subInstance == null) SpawnSub(spawnPoint);
            SubInstance.transform.position = spawnPoint;
        }

        private void OnLevelLoaded(Level level)
        {
            if (level == null)
            {
                Debug.LogError("No level was loaded!");
                return;
            }
            
            ResetPlayerEntities();
            
        }
    }
}