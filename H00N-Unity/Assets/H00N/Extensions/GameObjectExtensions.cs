using UnityEngine;

namespace H00N.Extensions
{
    public static class GameObjectExtensions
    {
        public static TComponent GetOrAddComponent<TComponent>(this GameObject self) where TComponent : Component
        {
            if(!self.TryGetComponent<TComponent>(out var component))
                component = self.AddComponent<TComponent>();

            return component;
        }
    }
}