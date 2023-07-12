using System.Collections.Generic;
using UnityEngine;

namespace ObjectPooling
{
    public class ObjectPool
    {
        // Private fields
        private readonly GameObject _prefab;
        private readonly int _poolSize;
        [SerializeField] private readonly List<GameObject> _poolObjects;
        private readonly Transform _container;

        // Public Properties
        public List<GameObject> PoolObjects => _poolObjects;

        // Constructor
        public ObjectPool(GameObject prefab, int initialPoolSize, Transform container = null)
        {
            _prefab = prefab;
            _poolSize = initialPoolSize;
            _container = container == null ? PoolManager.Instance.transform : container;

            // Create the pool objects
            _poolObjects = new List<GameObject>();
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject obj = InstantiatePrefab();
                obj.SetActive(false);
                _poolObjects.Add(obj);
            }
        }

        // Spawn an object from the pool
        public GameObject SpawnFromPool(Vector3 position, Quaternion rotation)
        {
            for (int i = 0; i < _poolObjects.Count; i++)
            {
                if (_poolObjects[i].activeInHierarchy) continue;

                _poolObjects[i].transform.position = position;
                _poolObjects[i].transform.rotation = rotation;
                _poolObjects[i].SetActive(true);
                return _poolObjects[i];
            }

            // If all objects are active, create a new one and add it to the pool
            GameObject newObj = InstantiatePrefab();
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;
            newObj.SetActive(true);
            _poolObjects.Add(newObj);

            return newObj;
        }

        // Return an object to the pool
        public void ReturnToPool(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

        // Instantiate the prefab and set its parent to the pool manager
        private GameObject InstantiatePrefab()
        {
            return Object.Instantiate(_prefab, _container, true);
        }
    }

}