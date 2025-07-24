using UnityEngine;

namespace H00N.Resources
{
    public class ResourceHandle
    {
        private string resourceName = null;
        public string ResourceName => resourceName;

        private Object resource = null;

        public ResourceHandle(string resourceName, Object resource)
        {
            this.resourceName = resourceName;
            this.resource = resource;
        }

        public Object Get() => resource;
        public T Get<T>() where T : Object
        {
            if(typeof(T).IsSubclassOf(typeof(Component)) && resource is GameObject gameObject)
                return gameObject.GetComponent<T>();
            else
                return resource as T;
        }

        public virtual void Initialize() {}
        public virtual void Release() 
        {
            resource = null;
            resourceName = null;
        }
    }
}