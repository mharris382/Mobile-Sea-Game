using System;
using UnityEngine;

namespace Utilities.Pools
{
    public class PooledMonoBehaviour : MonoBehaviour
    {
        [SerializeField] private int _initialPoolSize = 100;
        public int InitialPoolSize => _initialPoolSize;

        public event Action OnDisableEvent;

        protected virtual void OnDisable()
        {
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
                pooledObject.transform.localPosition = Vector3.zero;
                pooledObject.transform.localRotation = Quaternion.identity;
            }

            return pooledObject;
        }

        public T Get<T>(Transform parent, Vector3 relativePosition, Quaternion relativeRotation)
            where T : PooledMonoBehaviour
        {
            var pooledObject = Get<T>(true);
            pooledObject.transform.SetParent(parent);

            pooledObject.transform.localPosition = relativePosition;
            pooledObject.transform.localRotation = relativeRotation;

            return pooledObject;
        }
    }
}