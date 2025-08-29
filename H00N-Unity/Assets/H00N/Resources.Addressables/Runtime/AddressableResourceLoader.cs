using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace H00N.Resources.Addressables
{
    public class AddressableResourceLoader : IResourceLoader
    {
        public async UniTask<ResourceHandle> LoadResourceAsync<T>(string resourceName) where T : Object
        {
            if(typeof(T).IsSubclassOf(typeof(Component)))
                return await LoadResourceInternal<GameObject>(resourceName);
            else
                return await LoadResourceInternal<T>(resourceName);
        }

        private async UniTask<ResourceHandle> LoadResourceInternal<T>(string resourceName) where T : Object
        {
            try {
                AsyncOperationHandle<T> requestHandle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(resourceName);
                await requestHandle.Task;

                if(requestHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning($"[Addressable] Failed to load resource. : {resourceName}");
                    return null;
                }
                
                ResourceHandle resourceHandle = new AddressableResourceHandle(resourceName, requestHandle.Result, requestHandle);
                return resourceHandle;
            } 
            catch(Exception err) {
                Debug.LogWarning(err);
                return null;
            }
        }

        public static UniTask LoadResourcesByLabelAsync(string label, List<string> resources = null) => LoadResourcesByLabelAsync<Object>(label, resources);
        public static async UniTask LoadResourcesByLabelAsync<T>(string label, List<string> resources = null) where T : Object
        {
            IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> locations = await UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync(label, typeof(T));
            UniTask[] tasks = new UniTask[locations.Count];
            for(int i = 0; i < locations.Count; i++)
            {
                string resourceName = locations[i].PrimaryKey;
                resources?.Add(resourceName);
                tasks[i] = ResourceManager.LoadResourceAsync<T>(resourceName);
            }

            await UniTask.WhenAll(tasks);
        }

        public static UniTask ReleaseResourcesByLabelAsync(string label, List<string> resources = null) => ReleaseResourcesByLabelAsync<Object>(label, resources);
        public static async UniTask ReleaseResourcesByLabelAsync<T>(string label, List<string> resources = null) where T : Object
        {
            IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> locations = await UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync(label, typeof(T));
            for(int i = 0; i < locations.Count; i++)
            {
                string resourceName = locations[i].PrimaryKey;
                resources?.Add(resourceName);
                ResourceManager.ReleaseResource(resourceName);
            }
        }
    }
}