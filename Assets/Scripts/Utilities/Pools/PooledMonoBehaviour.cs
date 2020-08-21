using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utilities.Pools
{
    public class PooledMonoBehaviour : MonoBehaviour
    {
        [HideInPrefabAssets]
        public bool isSceneInstance = false;
        [HideIf("isSceneInstance")]
        [SerializeField] private int _initialPoolSize = 100;
        public int InitialPoolSize => _initialPoolSize;

        public event Action OnDisableEvent;

        protected virtual void OnDisable()
        {
            if (isSceneInstance) return;
            OnDisableEvent?.Invoke();
        }

        public T Get<T>(bool enable = true) where T : PooledMonoBehaviour
        {
            var pool = Pool.GetPool(this);
            var pooledObject = pool.Get<T>();

            if (enable)
            {
                pooledObject.gameObject.SetActive(true);
            }

            return pooledObject;
        }

        public T Get<T>(Transform parent, bool resetTransform = false) where T : PooledMonoBehaviour
        {
            var pooledObject = Get<T>(true);
            pooledObject.transform.SetParent(parent);

            if (resetTransform)
            {
                var transform1 = pooledObject.transform;
                transform1.localPosition = Vector3.zero;
                transform1.localRotation = Quaternion.identity;
            }

            return pooledObject;
        }

        public T Get<T>(Vector3 position, Quaternion rotation, Transform parent)
            where T : PooledMonoBehaviour
        {
            var pooledObject = Get<T>(true);
            
            Transform transform1 =pooledObject.transform;
            
            
            transform1.position = position;
            transform1.rotation = rotation;
            transform1.parent = parent;
            
            return pooledObject;
        }
    }
}