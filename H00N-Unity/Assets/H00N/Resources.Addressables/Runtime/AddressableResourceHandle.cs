using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace H00N.Resources.Addressables
{
    public class AddressableResourceHandle : ResourceHandle
    {
        private AsyncOperationHandle operationHandle;

        public AddressableResourceHandle(string resourceName, Object resource, AsyncOperationHandle operationHandle) : base(resourceName, resource)
        {
            this.operationHandle = operationHandle;
        }

        public override void Release()
        {
            UnityEngine.AddressableAssets.Addressables.Release(operationHandle);
            base.Release();
        }
    }
}