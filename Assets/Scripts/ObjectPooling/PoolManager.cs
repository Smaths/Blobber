using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Utility;

namespace ObjectPooling
{
    public class PoolManager : Singleton<PoolManager>
    {
        private Dictionary<string, ObjectPool> _pools;

        protected override void Awake()
        {
            base.Awake();

            // Initialize the dictionary
            _pools = new Dictionary<string, ObjectPool>();
        }

        // Create an object pool for a specific prefab
        public void CreatePool(GameObject prefab, int initialPoolSize, Transform container = null)
        {
            if (_pools.ContainsKey(prefab.name)) return;

            var pool = new ObjectPool(prefab, initialPoolSize, container);
            _pools.Add(prefab.name, pool);
        }

        // Spawn an object from the pool
        public GameObject SpawnFromPool(GameObject prefab, Vector3 position)
        {
            return SpawnFromPool(prefab, position, quaternion.identity);
        }

        public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (_pools.TryGetValue(prefab.name, out ObjectPool pool)) return pool.SpawnFromPool(position, rotation);

#if UNITY_EDITOR
            Debug.LogWarning("No object pool found for " + prefab.name);
#endif
            return null;
        }

        /// <summary>
        ///     Return an object to the pool.
        /// </summary>
        /// <param name="prefab">Name used as key for accessing the correct pool.</param>
        /// <param name="go">Object to be returned to pool.</param>
        public void ReturnToPool(GameObject prefab, GameObject go)
        {
            if (_pools.TryGetValue(prefab.name, out ObjectPool pool))
            {
                pool.ReturnToPool(go);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("No object pool found for " + prefab.name);
#endif
            }
        }

        public List<GameObject> GetPoolObjects(GameObject prefab)
        {
            if (_pools.TryGetValue(prefab.name, out ObjectPool pool))
                return pool.PoolObjects;

#if UNITY_EDITOR
            Debug.LogWarning("No object pool found for " + prefab.name);
#endif
            return null;
        }
    }
}