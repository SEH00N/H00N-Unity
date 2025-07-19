using System;
using System.Collections.Generic;

namespace H00N.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> callback)
        {
            IEnumerator<T> enumerator = self.GetEnumerator();
            while(enumerator.MoveNext())
                callback?.Invoke(enumerator.Current);
        }
    }
}