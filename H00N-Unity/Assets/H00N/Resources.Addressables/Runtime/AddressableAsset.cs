using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace H00N.Resources.Addressables
{
    [System.Serializable]
    public class AddressableAsset<T> where T : Object
    {
        [SerializeField] string key = null;
        public string Key => key;

        private T asset = null;
        public T Asset => asset;

        public async UniTask InitializeAsync()
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Key is null or empty.");
                return;
            }

            asset = await ResourceManager.LoadResourceAsync<T>(key);
        }

        public void Release()
        {
            if (string.IsNullOrEmpty(key))
                return;

            ResourceManager.ReleaseResource(key);
            asset = null;
        }

        public static implicit operator T(AddressableAsset<T> reference) => reference.Asset;
        public static implicit operator string(AddressableAsset<T> reference) => reference.Key;
    }
}
