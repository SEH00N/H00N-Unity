using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;

namespace H00N.Resources
{
    public static class ResourceManager
    {
        private static IResourceLoader resourceLoader = null;
        private static Dictionary<string, ResourceHandle> resourceCache = null;
        private static HashSet<string> loadingResourceKeys = null;

        private static bool initialized = false;
        public static bool Initialized => initialized;

        public static void Initialize(IResourceLoader loader)
        {
            resourceLoader = loader;
            resourceCache = new Dictionary<string, ResourceHandle>();
            loadingResourceKeys = new HashSet<string>();

            initialized = true;
        }

        public static void Release()
        {
            resourceLoader = null;
            foreach(ResourceHandle handle in resourceCache.Values)
                handle?.Release();
            resourceCache = null;

            initialized = false;
        }

        public static T GetResource<T>(string resourceName) where T : Object
        {
            if(string.IsNullOrEmpty(resourceName))
                return null;

            if (resourceCache.TryGetValue(resourceName, out ResourceHandle resourceHandle) == false)
            {
                Debug.LogWarning($"[ResourceManager::GerResource] Resource handle does not exist. Load resource handle before access resource. ResourceName : {resourceName}");
                return null;
            }

            if(resourceHandle == null)
            {
                Debug.LogWarning($"[ResourceManager::GerResource] Resource handle is null. ResourceName : {resourceName}");
                return null;
            }

            T resource = resourceHandle.Get<T>();
            if (resource == null)
            {
                Debug.LogWarning($"[ResourceManager::GerResource] Resource type does not match. ResourceName : {resourceName}, TypeName: {typeof(T).Name}");
                return null;
            }

            return resource;
        }

        public static UniTask<Object> LoadResourceAsync(string resourceName) => LoadResourceAsync<Object>(resourceName);
        public static async UniTask<T> LoadResourceAsync<T>(string resourceName) where T : Object
        {
            if(string.IsNullOrEmpty(resourceName))
                return null;

            ResourceHandle resourceHandle = await LoadResourceHandleAsync<T>(resourceName);
            if(resourceHandle == null)
                return null;

            T resource = resourceHandle.Get<T>();
            if (resource == null)
            {
                Debug.LogWarning($"[ResourceManager::LoadResourceAsync] Resource type does not match. ResourceName : {resourceName}, TypeName: {typeof(T).Name}");
                return null;
            }

            return resource;
        }

        private static async UniTask<ResourceHandle> LoadResourceHandleAsync<T>(string resourceName) where T : Object
        {
            if (string.IsNullOrEmpty(resourceName))
                return null;

            if (resourceCache.TryGetValue(resourceName, out ResourceHandle resourceHandle))
                return resourceHandle;

            if(loadingResourceKeys.Contains(resourceName))
            {
                do
                {
                    await UniTask.Delay(50);
                }
                while(loadingResourceKeys.Contains(resourceName));

                resourceCache.TryGetValue(resourceName, out resourceHandle);
                return resourceHandle;
            }

            loadingResourceKeys.Add(resourceName);
            resourceHandle = await resourceLoader.LoadResourceAsync<T>(resourceName);
            loadingResourceKeys.Remove(resourceName);

            if (resourceHandle == null)
                Debug.LogWarning($"[Resource] Resource not found. : {resourceName}");

            resourceCache.Add(resourceName, resourceHandle);
            return resourceHandle;
        }

        public static void ReleaseResource(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName))
                return;

            if (resourceCache.TryGetValue(resourceName, out ResourceHandle handle) == false)
                return;

            if(handle == null)
                return;

            handle.Release();
            resourceCache.Remove(resourceName);
        }

        public static async UniTask<ResourceHandle> LoadResourceWithoutCahingAsync<T>(string resourceName) where T : Object
        {
            if(string.IsNullOrEmpty(resourceName))
                return null;

            ResourceHandle handle = await resourceLoader.LoadResourceAsync<T>(resourceName);
            if(handle == null)
            {
                Debug.LogWarning($"[Resource] Resource not found. : {resourceName}");
                return null;
            }

            return handle;
        }
    }
}