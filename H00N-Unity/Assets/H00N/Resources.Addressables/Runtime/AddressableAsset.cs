using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace H00N.Resources.Addressables
{
    [System.Serializable]
    public class AddressableAsset<T> where T : Object
    {
        private enum EState
        {
            Uninitialized,
            Initializing,
            Initialized
        }

        [SerializeField] string key = null;
        public string Key => key;

        private T asset = null;
        public T Asset => asset;

        private EState state = EState.Uninitialized;
        public bool Initialized => state == EState.Initialized;

        public AddressableAsset()
        {
            state = EState.Uninitialized;
        }

        public async UniTask InitializeAsync()
        {
            if(Initialized)
                return;

            if(state == EState.Initializing)
            {
                await UniTask.WaitUntil(() => Initialized);
                return;
            }

            state = EState.Initializing;
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Key is null or empty.");
                state = EState.Initialized;
                return;
            }

            asset = await ResourceManager.LoadResourceAsync<T>(key);
            state = EState.Initialized;
        }

        public static implicit operator T(AddressableAsset<T> reference) => reference.Asset;
        public static implicit operator string(AddressableAsset<T> reference) => reference.Key;
    }
}
