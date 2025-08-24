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

        public static UniTask<List<string>> LoadResourcesByLabelAsync(string label) => LoadResourcesByLabelAsync<Object>(label);
        public static async UniTask<List<string>> LoadResourcesByLabelAsync<T>(string label) where T : Object
        {
            IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> locations = await UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync(label, typeof(T));
            List<string> resourceNames = new List<string>();
            for(int i = 0; i < locations.Count; i++)
            {
                string resourceName = locations[i].PrimaryKey;
                resourceNames.Add(resourceName);
                await ResourceManager.LoadResourceAsync<T>(resourceName);
            }

            return resourceNames;
        }

        public static UniTask<List<string>> ReleaseResourcesByLabelAsync(string label) => ReleaseResourcesByLabelAsync<Object>(label);
        public static async UniTask<List<string>> ReleaseResourcesByLabelAsync<T>(string label) where T : Object
        {
            IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> locations = await UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync(label, typeof(T));
            List<string> resourceNames = new List<string>();
            for(int i = 0; i < locations.Count; i++)
            {
                string resourceName = locations[i].PrimaryKey;
                resourceNames.Add(resourceName);
                ResourceManager.ReleaseResource(resourceName);
            }

            return resourceNames;
        }
    }
}