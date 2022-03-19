using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages an array of object pools.
/// </summary>
public class ObjectPool : Singleton<ObjectPool>
{
    #region VARIABLES =====

    public Pool[] pools;
    public Dictionary<string, Pool> poolDict;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab; // Prefab to pool.
        public int size = 10; // Amount to instantiate initially.
        public int refill = 0; // Additional prefab instances to instantiate when needed.
        public int maxSize = 30; // Max amount.
        public LinkedList<GameObject> pooledObjects = new LinkedList<GameObject>();
        private int count = 0;

        // Get and remove the first pooled object from linked list.
        public GameObject GrabFirst()
        {
            GameObject _go = pooledObjects.First.Value;
            pooledObjects.RemoveFirst();
            count -= 1;
            return _go;
        }

        // Add pooled object to the head of linked list.
        public void AddFirst(GameObject go)
        {
            pooledObjects.AddFirst(go);
            count += 1;
        }

        // Add pooled object to the tail of linked list.
        public void AddLast(GameObject go)
        {
            pooledObjects.AddLast(go);
            count += 1;
        }

        // Instantiate prefabs to refill pool.
        public void Refill()
        {
            if (refill > 0 && count + refill <= maxSize)
            {
                for (int i = 0; i < refill; i++)
                {
                    GameObject _newGo = Instantiate(prefab);
                    _newGo.SetActive(false);
                    AddFirst(_newGo);
                }
            }
        }

        public GameObject Peek() { return pooledObjects.First.Value; }
        public int GetCount() { return count; }
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        poolDict = new Dictionary<string, Pool>();
        foreach (Pool pool in pools)
        {
            // For each pool, populate a linked list with instantiated gameObjects;
            for (int i = 0; i < pool.size; i++)
            {
                GameObject _go = Instantiate(pool.prefab);
                _go.SetActive(false);
                pool.AddLast(_go);
            }

            // Add to tag <-> Pool dictionary.
            poolDict.Add(pool.tag, pool);
        }
    }

    // Call from other scripts to get a pooled GameObject.
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (poolDict.TryGetValue(tag, out Pool _pool))
        {
            // If next pooled object is active, we assume all pooled objects are now in use,
            //... so we do a refill.
            if (_pool.Peek().activeSelf == true)
                _pool.Refill();

            // "Dequeue" a GameObject.
            GameObject _obj = _pool.GrabFirst();

            if (_obj == null)
            {
                Debug.LogWarning("Pooled object" + tag + "is null");
                return null;
            }

            // Initialize its transform.
            if (parent != null)
                _obj.transform.SetParent(parent);
            _obj.transform.position = position;
            _obj.transform.rotation = rotation;
            //obj.SetActive(true);

            // "Queue" this GameObject to end of linked list.
            _pool.AddLast(_obj);

            return _obj;
        }
        else
        {
            Debug.LogWarning("Object pool with tag " + tag + " doesn't exist");
            return null;
        }
    }
}