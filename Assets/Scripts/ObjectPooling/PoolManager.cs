using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ObjectPooling
{
    public class PoolManager : MonoBehaviour
    {
        // Singleton instance
        public static PoolManager Instance;

        private Dictionary<string, ObjectPool> _pools;

        private void Awake()
        {
            // Create the singleton instance
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

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
            if (_pools.TryGetValue(prefab.name, out ObjectPool pool))
            {
                return pool.SpawnFromPool(position, rotation);
            }

            #if UNITY_EDITOR
            Debug.LogWarning("No object pool found for " + prefab.name);
            #endif
            return null;
        }

        // Return an object to the pool
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
    }

}