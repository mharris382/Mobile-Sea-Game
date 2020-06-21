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
        // [SerializeField] private DiverConfig diverConfig;
        [SerializeField] private GameObject diver;
        //[SerializeField] private SubConfig subConfig;
        [SerializeField] private GameObject sub;
        public CinemachineVirtualCamera diverCam;
        public PolyNav2D map;


        private static PlayerController _instance;

        public static PlayerController Instance => _instance;

   
        public GameObject _subInstance => sub;
        public GameObject _diverInstance => diver;
        
        [System.Obsolete]
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

        [System.Obsolete]
        private void SpawnDiver(Vector3 diverSpawnPosition)
        {
            if (_diverInstance != null) GameObject.Destroy(_diverInstance.gameObject);
            diverSpawnPosition.z = 0;
            //_diverInstance.SendMessage("SetConfig", DiverConfig, SendMessageOptions.RequireReceiver);
            diverCam.m_Follow = _diverInstance.transform;
        }

        [System.Obsolete]
        private void SpawnSub(Vector3 subSpawnPosition)
        {
            if (_subInstance != null) GameObject.Destroy(_subInstance.gameObject);
            subSpawnPosition.z = 0;
            if (SubNavTarget == null)
            {
                var go = new GameObject("Sub Nav Goal");
                SubNavTarget = go.transform;
            }

            SubNavTarget.position = subSpawnPosition + Vector3.right + (Vector3.down / 2f);

            // _subInstance.SendMessage("SetConfig", SubConfig, SendMessageOptions.RequireReceiver);
            // _subInstance.SendMessage("SetTarget", SubNavTarget, SendMessageOptions.RequireReceiver);
            // if (map == null)
            // {
            //     var mgo = GameObject.FindGameObjectWithTag("SubNavMap");
            //     Debug.Assert(mgo != null, "No Map Tag Found");
            //     map = mgo.GetComponent<PolyNav2D>();
            //     Debug.Assert(map != null, "No Map found");
            // }
            // _subInstance.SendMessage("SetMap", map, SendMessageOptions.RequireReceiver);
        }

        private void ResetPlayerEntities()
        {
            if (GameManager.Instance.CurrentLevel == null) return;

            Vector2 spawnPoint = GameManager.Instance.CurrentLevel.diverSpawnPosition;

            diver.transform.position = spawnPoint;

            spawnPoint += (Vector2.right * 3);

            // if (_subInstance == null) SpawnSub(spawnPoint);
            sub.transform.position = spawnPoint;
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