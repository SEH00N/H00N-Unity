using System.Collections.Generic;
using UnityEngine;

namespace H00N.Resources.Pools
{
    public static partial class PoolManager
    {
        private static Transform poolParent = null;
        private static Dictionary<string, Pool> poolTable = null;

        private static bool initialized = false;
        public static bool Initialized => initialized;

        public static void Initialize(Transform parent)
        {
            poolParent = parent;
            poolTable = new Dictionary<string, Pool>();

            initialized = true;
        }

        public static void Release()
        {
            poolParent = null;
            foreach(Pool pool in poolTable.Values)
                pool.Release();
            poolTable = null;

            initialized = false;
        }

        public static GameObject Spawn(PoolReference resource, Transform parent = null)
        {
            PoolReference instance = SpawnInternal(GetPool(resource), parent);
            if(instance == null)
                return null;

            return instance.gameObject;
        }

        public static GameObject Spawn(string resourceName, Transform parent = null)
        {
            PoolReference instance = SpawnInternal(GetPool(resourceName), parent);
            if(instance == null)
                return null;

            return instance.gameObject;
        }

        public static T Spawn<T>(PoolReference resource, Transform parent = null) where T : Component
        {
            PoolReference instance = SpawnInternal(GetPool(resource), parent);
            if(instance == null)
                return null;
            
            if (instance is T)
                return instance as T;

            return instance.GetComponent<T>();
        }

        public static T Spawn<T>(string resourceName, Transform parent = null) where T : Component
        {
            PoolReference instance = SpawnInternal(GetPool(resourceName), parent);
            if(instance == null)
                return null;
            
            if (instance is T)
                return instance as T;

            return instance.GetComponent<T>();
        }

        private static PoolReference SpawnInternal(Pool pool, Transform parent)
        {
            if(pool == null)
            {
                Debug.LogWarning($"[PoolManager::SpawnInternal] Pool not found.");
                return null;
            }

            PoolReference instance = pool.Spawn();
            if(instance == null)
                return null;

            instance.gameObject.SetActive(true);
            instance.transform.SetParent(parent);
            return instance;
        }

        private static Pool GetPool(string resourceName)
        {
            if(poolTable.TryGetValue(resourceName, out Pool pool))
                return pool;

            GameObject resource = ResourceManager.GetResource<GameObject>(resourceName);
            if(resource == null)
            {
                Debug.LogWarning($"[PoolManager::GetPool] Spawn failed. Resource not found. resourceName : {resourceName}");
                return null;
            }

            if(resource.TryGetComponent<PoolReference>(out PoolReference poolReference) == false)
            {
                Debug.LogWarning($"[PoolManager::GetPool] Spawn failed. Current resource is not a PoolReference. resourceName : {resourceName}");
                return null;
            }

            return GetPool(poolReference);
        }

        private static Pool GetPool(PoolReference resource)
        {
            string resourceName = resource.gameObject.name;
            if(poolTable.TryGetValue(resourceName, out Pool pool))
            {
                if(pool.Resource != resource)
                {
                    Debug.LogWarning($"[PoolManager::GetPool] Pool already exists with different resource. resourceName : {resourceName}");
                    return null;
                }

                return pool;
            }

            resource.Initialize(resourceName);

            pool = new Pool(resource);
            poolTable.Add(resourceName, pool);
            
            return pool;
        }

        public static void Despawn(IPoolableBehaviour instance) => Despawn(instance.PoolReference);
        public static void Despawn(PoolReference instance)
        {
            if(instance == null)
                return;

            if(instance.Key == null)
            {
                instance.Despawn();
                Object.Destroy(instance.gameObject);
                return;
            }

            if(poolTable.TryGetValue(instance.Key, out Pool pool) == false)
            {
                Debug.LogWarning($"[PoolManager::Despawn] Pool not found. instance.Key : {instance.Key}");
                instance.Despawn();
                Object.Destroy(instance.gameObject);
                return;
            }

            pool.Despawn(instance);
            if(instance == null)
                return;

            instance.gameObject.SetActive(false);
            instance.transform.SetParent(poolParent);
        }
    }
}