using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Pools
{
    



    public class Pool : MonoBehaviour
{
    private static Dictionary<PooledMonoBehaviour, Pool> _pools = new Dictionary<PooledMonoBehaviour, Pool>();
    
    private Queue<PooledMonoBehaviour> _readyObjects = new Queue<PooledMonoBehaviour>();
    private List<PooledMonoBehaviour> _disabledObjects = new List<PooledMonoBehaviour>();

    private PooledMonoBehaviour _prefab;

    public static Pool GetPool(PooledMonoBehaviour prefab)
    {
        if (_pools.ContainsKey(prefab))
            return _pools[prefab];
        
        var pool = new GameObject("Pool - " + prefab.name).AddComponent<Pool>();
        pool._prefab = prefab;

        pool.GrowPool();
        _pools.Add(prefab, pool);

        return pool;
    }
    
    public T Get<T>() where T : PooledMonoBehaviour
    {
        if (_readyObjects.Count == 0)
        {
            GrowPool();
        }

        var pooledObject = _readyObjects.Dequeue();

        return pooledObject as T;
    }

    private void GrowPool()
    {
        for (var i = 0; i < _prefab.InitialPoolSize; i++)
        {
            var pooledObject = Instantiate(_prefab);

            pooledObject.gameObject.name += " " + i;

            pooledObject.OnDisableEvent += () => AddObjectToAvailable(pooledObject);

            pooledObject.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        MakeDisabledObjectsChildren();
    }

    private void MakeDisabledObjectsChildren()
    {
        if (_disabledObjects.Count > 0)
        {
            foreach (var pooledObject in _disabledObjects)
            {
                if (pooledObject.gameObject.activeInHierarchy == false)
                {
                    pooledObject.transform.SetParent(transform);
                }
            }
            
            _disabledObjects.Clear();
        }
    }

    private void AddObjectToAvailable(PooledMonoBehaviour pooledObject)
    {
        _disabledObjects.Add(pooledObject);
        _readyObjects.Enqueue(pooledObject);
    }
}
}