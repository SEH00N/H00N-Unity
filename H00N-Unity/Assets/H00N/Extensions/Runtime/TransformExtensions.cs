using System.Collections.Generic;
using UnityEngine;

namespace H00N.Extensions
{
    public static class TransformExtensions
    {
        public static int DistanceCompare(this Transform transform, Transform a, Transform b)
        {
            float sqrDistanceA = (a.position - transform.position).sqrMagnitude;
            float sqrDistanceB = (b.position - transform.position).sqrMagnitude;
            return sqrDistanceA.CompareTo(sqrDistanceB);
        }

        public static void GetComponentsInChildren<T>(this Transform transform, List<T> result, bool includeSelf, bool recursive = true) where T : Component
        {
            if(includeSelf)
                result.AddRange(transform.GetComponents<T>());

            foreach(Transform child in transform)
            {
                result.AddRange(child.GetComponents<T>());
                if (recursive)
                    child.GetComponentsInChildren(result, false, true);
            }            
        }
    }
}