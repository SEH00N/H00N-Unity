using System.Collections.Generic;
using UnityEngine;

namespace H00N.Extensions
{
    public static class ListExtensions
    {
        public static void PickShuffle<T>(this List<T> source, int count)
        {
            if(source.Count <= 0)
                return;

            for(int i = 0; i < count; ++i)
            {
                int left = Random.Range(0, source.Count);
                int right = Random.Range(0, source.Count);
                (source[right], source[left]) = (source[left], source[right]);
            }
        }

        public static T PickRandom<T>(this List<T> source)
        {
            if(source.Count <= 0)
                return default;

            int index = Random.Range(0, source.Count);
            return source[index];
        }
    }
}